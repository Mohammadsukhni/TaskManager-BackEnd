using System.Net;

namespace TaskManager.Core.Exceptions
{
    public class TooManyRequestsException : ApiException
    {
        public TooManyRequestsException(string message, int retryAfterSeconds)
            : base(message, HttpStatusCode.TooManyRequests)
        {
            RetryAfterSeconds = retryAfterSeconds;
        }

        public int RetryAfterSeconds { get; }
    }
}
