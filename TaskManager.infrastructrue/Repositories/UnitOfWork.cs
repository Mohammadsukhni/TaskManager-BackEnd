using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;

            Users = new GenericRepository<User>(_context, currentUserService);
            Projects = new GenericRepository<Project>(_context, currentUserService);
            UserProjects = new GenericRepository<UserProject>(_context, currentUserService);
            Sprints = new GenericRepository<Sprint>(_context, currentUserService);
            WorkItems = new GenericRepository<WorkItem>(_context, currentUserService);
            WorkItemRelations = new GenericRepository<WorkItemRelation>(_context, currentUserService);
            Otps = new GenericRepository<Otp>(_context, currentUserService);
        }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Project> Projects { get; }

        public IGenericRepository<UserProject> UserProjects { get; }

        public IGenericRepository<Sprint> Sprints { get; }

        public IGenericRepository<WorkItem> WorkItems { get; }

        public IGenericRepository<WorkItemRelation> WorkItemRelations { get; }

        public IGenericRepository<Otp> Otps { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
