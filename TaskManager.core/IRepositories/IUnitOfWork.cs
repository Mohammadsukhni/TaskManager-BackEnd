using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;

public interface IUnitOfWork
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Project> Projects { get; }
    IGenericRepository<UserProject> UserProjects { get; }
    IGenericRepository<Sprint> Sprints { get; }
    IGenericRepository<WorkItem> WorkItems { get; }
    IGenericRepository<WorkItemRelation> WorkItemRelations { get; }

    Task<int> SaveChangesAsync();
}