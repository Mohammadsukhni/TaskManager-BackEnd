using System.Net;

namespace TaskManager.Core.Exceptions
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
