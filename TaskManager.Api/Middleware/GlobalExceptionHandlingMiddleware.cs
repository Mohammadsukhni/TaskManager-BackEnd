using System.Net;
using TaskManager.Core.Exceptions;

namespace TaskManager_p.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCode(exception);

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unhandled exception occurred.");
            else
                _logger.LogWarning(exception, "Request failed with a handled exception.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            if (exception is TooManyRequestsException tooManyRequestsException)
                context.Response.Headers["Retry-After"] =
                    tooManyRequestsException.RetryAfterSeconds.ToString();

            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = GetMessage(exception, statusCode)
            });
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ApiException apiException => apiException.StatusCode,
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetMessage(Exception exception, HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;
        }
    }
}
