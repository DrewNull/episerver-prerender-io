namespace PrerenderForEpiserver
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;

    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class PrerenderHttpModuleInitialization : IInitializableHttpModule
    {
        private const string PrerenderSectionKey = "prerender";
        private const string EscapedFragment = "_escaped_fragment_";

        private PrerenderConfigSection _prerenderConfig;

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void InitializeHttpEvents(HttpApplication application)
        {
            try
            {
                _prerenderConfig = ConfigurationManager.GetSection(PrerenderSectionKey) as PrerenderConfigSection;

                application.BeginRequest += this.BeginRequest;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        protected void BeginRequest(object sender, EventArgs e)
        {
            try
            {
                this.DoPrerender(sender as HttpApplication);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        private void DoPrerender(HttpApplication application)
        {
            var httpContext = application.Context;
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (this.ShouldShowPrerenderedPage(request) == false)
                return;

            var result = this.GetPrerenderedPageResponse(request);

            response.StatusCode = (int)result.StatusCode;

            // The WebHeaderCollection is horrible, so we enumerate like this!
            // We are adding the received headers from the prerender service
            for (var i = 0; i < result.Headers.Count; ++i)
            {
                var header = result.Headers.GetKey(i);
                var values = result.Headers.GetValues(i);

                if (values == null) continue;

                foreach (var value in values)
                {
                    response.Headers.Add(header, value);
                }
            }

            response.Write(result.ResponseBody);
            response.Flush();
            application.CompleteRequest();
        }

        private bool ShouldShowPrerenderedPage(HttpRequest request)
        {
            var url = request.Url;

            if (this.IsNaughtyUrl(url, request.UrlReferrer?.AbsoluteUri ?? string.Empty))
                return false;

            if (!this.IsNiceUrl(url))
                return false;

            if (this.HasEscapedFragment(request))
                return true;

            if (!this.IsSearchUserAgent(request.UserAgent))
                return false;

            if (this.IsIgnoredExtension(url))
                return false;

            return true;
        }

        private bool IsNaughtyUrl(Uri url, string referer)
        {
            return
                _prerenderConfig?.Naughtylist
                    ?.Any(item =>
                    {
                        var regex = new Regex(item);
                        return regex.IsMatch(url.AbsoluteUri) || (!string.IsNullOrWhiteSpace(referer) && regex.IsMatch(referer));
                    })
                    ?? false;
        }

        private bool IsNiceUrl(Uri url)
        {
            return
                _prerenderConfig?.Nicelist
                    ?.Any(item => new Regex(item).IsMatch(url.AbsoluteUri))
                    ?? true;
        }

        private bool HasEscapedFragment(HttpRequest request)
        {
            return request.QueryString.AllKeys.Contains(EscapedFragment);
        }

        private bool IsSearchUserAgent(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return false;

            var crawlerUserAgents = new List<string>(PrerenderDefaults.CrawlerUserAgents);

            if (_prerenderConfig?.CrawlerUserAgents?.Any() == true)
            {
                crawlerUserAgents.AddRange(_prerenderConfig.CrawlerUserAgents);
            }

            bool isInUserAgent = crawlerUserAgents.Any(x => userAgent.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

            return isInUserAgent;
        }

        private bool IsIgnoredExtension(Uri url)
        {
            var extensionsToIgnore = new List<string>(PrerenderDefaults.ExtensionsToIgnore);

            if (_prerenderConfig.ExtensionsToIgnore?.Any() == true)
            {
                extensionsToIgnore.AddRange(_prerenderConfig.ExtensionsToIgnore);
            }

            var isInResource = extensionsToIgnore.Any(item => url.AbsoluteUri.ToLower().Contains(item.ToLower()));

            return isInResource;
        }

        private ResponseResult GetPrerenderedPageResponse(HttpRequest request)
        {
            var apiUrl = this.GetApiUrl(request);
            var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            webRequest.Method = "GET";
            webRequest.UserAgent = request.UserAgent;
            webRequest.AllowAutoRedirect = false;

            // Set proxy
            if (!string.IsNullOrWhiteSpace(_prerenderConfig.Proxy?.Url))
            {
                webRequest.Proxy = new WebProxy(_prerenderConfig.Proxy.Url, _prerenderConfig.Proxy.Port);
            }

            // Set no-cache
            webRequest.Headers.Add("Cache-Control", "no-cache");
            webRequest.ContentType = "text/html";

            // Add our Prerender.io account key
            if (!string.IsNullOrWhiteSpace(_prerenderConfig.Token))
            {
                webRequest.Headers.Add("X-Prerender-Token", _prerenderConfig.Token);
            }

            try
            {
                // Get the web response and read content etc. if successful
                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                return new ResponseResult(webResponse.StatusCode, reader.ReadToEnd(), webResponse.Headers);
            }
            catch (WebException e)
            {
                // Handle response WebExceptions for invalid renders (404s, 504s etc.) - but we still want the content
                var reader = new StreamReader(e.Response.GetResponseStream(), Encoding.UTF8);
                return new ResponseResult(((HttpWebResponse)e.Response).StatusCode, reader.ReadToEnd(), e.Response.Headers);
            }
        }

        private string GetApiUrl(HttpRequest request)
        {
            string url = request.Url.AbsoluteUri;

            // For testing locally, check the X-Original-Host in case of a proxy like Ngrok:
            if (request.IsLocal)
            {
                string originalHost = request.Headers["X-Original-Host"];
                if (!string.IsNullOrWhiteSpace(originalHost))
                {
                    url = $"{request.Url.Scheme}://{originalHost}{request.Url.PathAndQuery}";
                }
            }

            // request.RawUrl have the _escaped_fragment_ query string
            // Prerender server remove it before making a request, but caching plugins happen before prerender server remove it
            url = this.RemoveQueryStringByKey(url, EscapedFragment);

            // Correct for HTTPS if that is what the request arrived at the load balancer as
            // (AWS and some other load balancers hide the HTTPS from us as we terminate SSL at the load balancer!)
            if (string.Equals(request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
            {
                url = url.Replace("http://", "https://");
            }

            // Remove the application from the URL
            if (_prerenderConfig.StripApplicationNameFromRequestUrl
                && !string.IsNullOrEmpty(request.ApplicationPath)
                && request.ApplicationPath != "/")
            {
                // http://test.com/MyApp/?_escape_=/somewhere
                url = url.Replace(request.ApplicationPath, string.Empty);
            }

            string prerenderServiceUrl = _prerenderConfig.PrerenderServiceUrl;

            return
                prerenderServiceUrl.EndsWith("/")
                    ? prerenderServiceUrl + url
                    : $"{prerenderServiceUrl}/{url}";
        }

        private string RemoveQueryStringByKey(string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return
                newQueryString.Count > 0
                ? $"{pagePathWithoutQueryString}?{newQueryString}"
                : pagePathWithoutQueryString;
        }
    }
}
