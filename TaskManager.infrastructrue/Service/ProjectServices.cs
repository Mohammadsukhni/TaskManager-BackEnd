using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Exceptions;
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
        }

        public async Task<PagedResultDto<ProjectDto>> GetAllProjectsAsync(
            int pageNumber,
            int pageSize)
        {
            var projects = await _unitOfWork.Projects.GetPagedAsync(pageNumber, pageSize);

            return projects.ToPagedDto(x => x.ToDto());
        }

        public async Task<ProjectFilterResultDto> FilterProjectsAsync(
            string? search,
            int pageNumber,
            int pageSize)
        {
            var term = NormalizeSearch(search);

            var projects = string.IsNullOrWhiteSpace(term)
                ? await _unitOfWork.Projects.GetPagedAsync(pageNumber, pageSize)
                : await _unitOfWork.Projects.GetPagedAsync(
                    x => x.Name.Contains(term) ||
                         x.ReferenceNumber.Contains(term),
                    pageNumber,
                    pageSize);

            var projectIds = projects.Items.Select(x => x.Id).ToList();

            var sprints = projectIds.Count == 0
                ? new List<Sprint>()
                : await _unitOfWork.Sprints.GetAllAsync(x => projectIds.Contains(x.ProjectId));

            var sprintIds = sprints.Select(x => x.Id).ToList();

            var workItems = sprintIds.Count == 0
                ? new List<WorkItem>()
                : await _unitOfWork.WorkItems.GetAllAsync(x => sprintIds.Contains(x.SprintId));

            return new ProjectFilterResultDto
            {
                Projects = projects.ToPagedDto(x => x.ToDto()),
                Sprints = sprints.Select(x => x.ToDto()).ToList(),
                WorkItems = workItems.Select(x => x.ToDto()).ToList()
            };
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
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
                return;

            _unitOfWork.Projects.Delete(project);
        }

        public async Task AssignUserToProjectAsync(int projectId, int userId)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (project == null)
                throw new BadRequestException("Project not found.");

            if (user == null)
                throw new BadRequestException("User not found.");

            if (user.UserRole != UserRole.User)
                throw new BadRequestException("Only regular users can be assigned.");

            if (!user.IsActive)
                throw new BadRequestException("Cannot assign a deactivated user.");

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
        }

        public async Task<PagedResultDto<ProjectDto>> GetUserProjectsAsync(
            int userId,
            int pageNumber,
            int pageSize)
        {
            var userProjects = await _unitOfWork.UserProjects.GetAllAsync();

            var projectIds = userProjects
                .Where(x => x.UserId == userId)
                .Select(x => x.ProjectId)
                .ToList();

            var projects = await _unitOfWork.Projects.GetPagedAsync(
                x => projectIds.Contains(x.Id),
                pageNumber,
                pageSize);

            return projects.ToPagedDto(x => x.ToDto());
        }

        private static string NormalizeSearch(string? search)
        {
            return search?.Trim() ?? string.Empty;
        }
    }
}
