namespace PrerenderForEpiserver
{
    using System.Net;

    public class ResponseResult
    {
        public ResponseResult(HttpStatusCode code, string body, WebHeaderCollection headers)
        {
            this.StatusCode = code;
            this.ResponseBody = body;
            this.Headers = headers;
        }

        public HttpStatusCode StatusCode
        {
            get;
        }

        public string ResponseBody
        {
            get;
        }

        public WebHeaderCollection Headers
        {
            get;
        }
    }
}