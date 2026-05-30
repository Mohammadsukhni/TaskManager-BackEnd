using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Helper;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Core.Mapping;

namespace TaskManager.Infrastructure.Service
{
    public class ProjectServices : IProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateProjectAsync(ProjectDto dto)
        {
            var project = dto.ToEntity();

            project.ReferenceNumber =
                ReferenceNumberHelper.GenerateProjectReference();

            await _unitOfWork.Projects.CreateAsync(project);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();

            return projects.Select(x => x.ToDto()).ToList();
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
                return null;

            return project.ToDto();
        }

        public async Task UpdateProjectAsync(ProjectDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(dto.Id);

            if (project == null)
                return;

            project.Name = dto.Name;

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
                return;

            _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignUserToProjectAsync(int projectId, int userId)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (project == null)
                throw new InvalidOperationException("Project not found.");

            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (user.UserRole != UserRole.User)
                throw new InvalidOperationException("Only regular users can be assigned.");

            if (!user.IsActive)
                throw new InvalidOperationException("Cannot assign a deactivated user.");

            var exists = (await _unitOfWork.UserProjects.GetAllAsync())
                .Any(x => x.ProjectId == projectId &&
                          x.UserId == userId);

            if (exists)
                return;

            var userProject = new UserProject
            {
                ProjectId = projectId,
                UserId = userId
            };

            await _unitOfWork.UserProjects.CreateAsync(userProject);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ProjectDto>> GetUserProjectsAsync(int userId)
        {
            var userProjects = await _unitOfWork.UserProjects.GetAllAsync();

            var projectIds = userProjects
                .Where(x => x.UserId == userId)
                .Select(x => x.ProjectId)
                .ToList();

            var projects = await _unitOfWork.Projects.GetAllAsync();

            return projects
                .Where(x => projectIds.Contains(x.Id))
                .Select(x => x.ToDto())
                .ToList();
        }
    }
}
