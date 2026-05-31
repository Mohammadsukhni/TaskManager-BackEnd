using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManager.Core.Exceptions;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!int.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedException("User is not authenticated.");

            return parsedUserId;
        }

        public int? GetAuditUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return null;

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : null;
        }
    }
}
