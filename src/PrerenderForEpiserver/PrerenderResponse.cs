namespace PrerenderForEpiserver
{
    using System.Net;

    public class PrerenderResponse
    {
        public PrerenderResponse(HttpStatusCode code, string body, WebHeaderCollection headers)
        {
            this.Headers = headers;
            this.ResponseBody = body;
            this.StatusCode = code;
        }

        public WebHeaderCollection Headers { get; }

        public string ResponseBody { get; }

        public HttpStatusCode StatusCode { get; }
    }
}