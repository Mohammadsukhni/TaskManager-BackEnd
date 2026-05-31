namespace TaskManager.Core.IService
{
    public interface ILoginAttemptService
    {
        void EnsureLoginAllowed(string email);
        void RegisterFailedAttempt(string email);
        void ResetFailedAttempts(string email);
    }
}
