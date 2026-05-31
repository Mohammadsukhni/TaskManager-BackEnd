using TaskManager.Infrastructure.Data;

namespace TaskManager_p.Middleware
{
    public class SaveChangesMiddleware
    {
        private readonly RequestDelegate _next;

        public SaveChangesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            context.Response.OnStarting(async state =>
            {
                var httpContext = (HttpContext)state;

                if (httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest)
                    return;

                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync(httpContext.RequestAborted);
            }, context);

            await _next(context);
        }
    }
}
