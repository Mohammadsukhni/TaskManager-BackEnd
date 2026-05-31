namespace TaskManager.Core.IService
{
    public interface ICurrentUserService
    {
        int GetUserId();
        int? GetAuditUserId();
    }
}
