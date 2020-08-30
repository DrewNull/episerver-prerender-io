namespace PrerenderForEpiserver
{
    using System;
    using System.Linq;
    using System.Web;

    public static class PrerenderExtensions
    {
        public static bool ShouldShowPrerenderedPage(this HttpRequest request)
        {
            var url = request.Url;

            if (request.HasEscapedFragment())
                return true;

            if (request.IsSearchUserAgent() == false)
                return false;

            if (url.IsIgnoredExtension())
                return false;

            return true;
        }

        public static bool HasEscapedFragment(this HttpRequest request)
        {
            bool hasEscapedFragment = request.QueryString.AllKeys.Contains(PrerenderConstants.EscapedFragmentQueryParameter);

            return hasEscapedFragment;
        }

        public static bool IsSearchUserAgent(this HttpRequest request)
        {
            string userAgent = request.UserAgent;

            if (string.IsNullOrWhiteSpace(userAgent))
                return false;

            bool isInUserAgent = 
                PrerenderConstants.CrawlerUserAgents
                    .Any(x => userAgent.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

            return isInUserAgent;
        }

        public static bool IsIgnoredExtension(this Uri url)
        {
            bool isInResource =
                PrerenderConstants.ExtensionsToIgnore
                    .Any(item => url.AbsoluteUri.ToLower().Contains(item.ToLower()));

            return isInResource;
        }
    }
}