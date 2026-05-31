using System.Net;

namespace TaskManager.Core.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message)
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}
