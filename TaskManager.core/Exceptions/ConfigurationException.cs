using System.Net;

namespace TaskManager.Core.Exceptions
{
    public class ConfigurationException : ApiException
    {
        public ConfigurationException(string message)
            : base(message, HttpStatusCode.InternalServerError)
        {
        }
    }
}
