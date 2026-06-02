using TaskManager.Core.Entities;

namespace TaskManager.Core.IRepositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Project> Projects { get; }
        IGenericRepository<UserProject> UserProjects { get; }
        IGenericRepository<Sprint> Sprints { get; }
        IGenericRepository<WorkItem> WorkItems { get; }
        IGenericRepository<WorkItemRelation> WorkItemRelations { get; }
        IGenericRepository<Otp> Otps { get; }

        Task<int> SaveChangesAsync();
    }
}
