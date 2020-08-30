namespace PrerenderForEpiserver
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using EPiServer;

    public class PrerenderHandler
    {
        public void DoPrerender(HttpApplication application)
        {
            var request = application.Context.Request;
            var response = application.Context.Response;

            if (request.ShouldShowPrerenderedPage() == false)
                return;

            var prerenderResponse = this.GetPrerenderResponse(request);

            response.StatusCode = (int)prerenderResponse.StatusCode;

            // Add the headers received from the Prerender service
            for (int index = 0; index < prerenderResponse.Headers.Count; ++index)
            {
                string headerName = prerenderResponse.Headers.GetKey(index);
                var headerValues = prerenderResponse.Headers.GetValues(index);

                if (headerValues == null) 
                    continue;

                foreach (string headerValue in headerValues)
                {
                    response.Headers.Add(headerName, headerValue);
                }
            }

            response.Write(prerenderResponse.ResponseBody);
            response.Flush();
            application.CompleteRequest();
        }

        private PrerenderResponse GetPrerenderResponse(HttpRequest request)
        {
            string prerenderUrl = this.GetPrerenderPageUrl(request);
            var webRequest = (HttpWebRequest)WebRequest.Create(prerenderUrl);
            webRequest.Method = "GET";
            webRequest.UserAgent = request.UserAgent;
            webRequest.AllowAutoRedirect = false;
            webRequest.Headers.Add("Cache-Control", "no-cache");
            webRequest.ContentType = "text/html";

            // Add our Prerender.io account key
            string token = ConfigurationManager.AppSettings[PrerenderConstants.TokenAppSetting];

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException($"A Prerender.io account token is required. Make sure it has been added to the Prerender:Token appSetting.");
            }
            
            webRequest.Headers.Add("X-Prerender-Token", token);

            try
            {
                // Get the web response and read content etc. if successful
                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                return new PrerenderResponse(webResponse.StatusCode, reader.ReadToEnd(), webResponse.Headers);
            }
            catch (WebException e)
            {
                // Handle response WebExceptions for invalid renders (404s, 504s etc.) - but we still want the content
                var reader = new StreamReader(e.Response.GetResponseStream(), Encoding.UTF8);
                return new PrerenderResponse(((HttpWebResponse)e.Response).StatusCode, reader.ReadToEnd(), e.Response.Headers);
            }
        }

        private string GetPrerenderPageUrl(HttpRequest request)
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

            // Remove the escaped fragment URL query parameter if it exists
            var urlBuilder = new UrlBuilder(url);
            urlBuilder.QueryCollection.Remove(PrerenderConstants.EscapedFragmentQueryParameter);
            url = urlBuilder.ToString();

            // Correct for HTTPS if that is what the request arrived at the load balancer as
            // (AWS and some other load balancers hide the HTTPS from us as we terminate SSL at the load balancer!)
            if (string.Equals(request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
            {
                url = url.Replace("http://", "https://");
            }

            string prerenderServiceUrl = PrerenderConstants.ServiceUrl;

            string prerenderUrl = 
                prerenderServiceUrl.EndsWith("/")
                    ? prerenderServiceUrl + url
                    : $"{prerenderServiceUrl}/{url}";

            return prerenderUrl;
        }
    }
}