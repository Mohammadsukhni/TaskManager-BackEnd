using Microsoft.Extensions.Caching.Memory;
using TaskManager.Core.Exceptions;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class LoginAttemptService : ILoginAttemptService
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan FailedAttemptWindow = TimeSpan.FromMinutes(5);
        private readonly IMemoryCache _cache;

        public LoginAttemptService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void EnsureLoginAllowed(string email)
        {
            var key = GetCacheKey(email);

            if (!_cache.TryGetValue<LoginAttemptState>(key, out var state) ||
                state?.LockedUntil == null)
            {
                return;
            }

            if (state.LockedUntil > DateTime.UtcNow)
                throw CreateLockoutException(state.LockedUntil.Value);

            _cache.Remove(key);
        }

        public void RegisterFailedAttempt(string email)
        {
            var key = GetCacheKey(email);

            _cache.TryGetValue<LoginAttemptState>(key, out var state);

            var failedAttempts = (state?.FailedAttempts ?? 0) + 1;

            if (failedAttempts >= MaxFailedAttempts)
            {
                var lockedUntil = DateTime.UtcNow.Add(LockoutDuration);

                _cache.Set(
                    key,
                    new LoginAttemptState(failedAttempts, lockedUntil),
                    LockoutDuration);

                throw CreateLockoutException(lockedUntil);
            }

            _cache.Set(
                key,
                new LoginAttemptState(failedAttempts, null),
                FailedAttemptWindow);
        }

        public void ResetFailedAttempts(string email)
        {
            _cache.Remove(GetCacheKey(email));
        }

        private static TooManyRequestsException CreateLockoutException(DateTime lockedUntil)
        {
            var retryAfterSeconds = Math.Max(
                1,
                (int)Math.Ceiling((lockedUntil - DateTime.UtcNow).TotalSeconds));

            return new TooManyRequestsException(
                $"Too many failed login attempts. Try again after {retryAfterSeconds} seconds.",
                retryAfterSeconds);
        }

        private static string GetCacheKey(string email)
        {
            return $"login-attempt:{email.Trim().ToLowerInvariant()}";
        }

        private sealed class LoginAttemptState
        {
            public LoginAttemptState(int failedAttempts, DateTime? lockedUntil)
            {
                FailedAttempts = failedAttempts;
                LockedUntil = lockedUntil;
            }

            public int FailedAttempts { get; }
            public DateTime? LockedUntil { get; }
        }
    }
}
