using System.Net;
using System.Net.Http;

namespace AzureIndexer.Api.Infrastructure
{
    using System;

    public class HttpResponseException : Exception
    {
        public HttpResponseException(string message, HttpStatusCode httpStatusCode)
            : base(message)
        {
            this.StatusCode = httpStatusCode;
        }

        public HttpResponseException(HttpResponseMessage message)
            : base(message.ReasonPhrase)
        {
            this.StatusCode = message.StatusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}
