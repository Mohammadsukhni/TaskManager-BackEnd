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

        public async Task<PagedResultDto<ProjectDto>> GetAllProjectsAsync(int pageNumber, int pageSize)
        {
            var projects = await _unitOfWork.Projects.GetPagedAsync(pageNumber, pageSize);
            var result = projects.ToPagedDto(x => x.ToDto());

            await AttachAssignedUsersAsync(result.Items);

            return result;
        }

        public async Task<ProjectFilterResultDto> FilterProjectsAsync(string? search, int pageNumber, int pageSize)
        {
            var term = NormalizeSearch(search);
            var projects = string.IsNullOrWhiteSpace(term)
                ? await _unitOfWork.Projects.GetPagedAsync(pageNumber, pageSize)
                : await _unitOfWork.Projects.GetPagedAsync(
                    x => x.Name.Contains(term) ||
                         x.ReferenceNumber.Contains(term),
                    pageNumber,
                    pageSize);

            var sprints = await GetProjectSprintsAsync(projects.Items);
            var workItems = await GetSprintWorkItemsAsync(sprints);
            var projectDtos = projects.ToPagedDto(x => x.ToDto());

            await AttachAssignedUsersAsync(projectDtos.Items);

            return new ProjectFilterResultDto
            {
                Projects = projectDtos,
                Sprints = sprints.Select(x => x.ToDto()).ToList(),
                WorkItems = workItems.Select(x => x.ToDto()).ToList()
            };
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
                return null;

            var dto = project.ToDto();

            await AttachAssignedUsersAsync(new[] { dto });

            return dto;
        }

        public async Task UpdateProjectAsync(ProjectDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(dto.Id);

            if (project == null)
                return;

            project.Name = dto.Name;

            await _unitOfWork.Projects.Update(project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
                return;

            await _unitOfWork.Projects.Delete(project);
        }

        public async Task AssignUserToProjectAsync(int projectId, int userId)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);

            if (project == null)
                throw new BadRequestException("Project not found.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            EnsureAssignableUser(user);

            var existingAssignments = await _unitOfWork.UserProjects.GetAllAsync(
                x => x.ProjectId == projectId &&
                     x.UserId == userId);

            if (existingAssignments.Count > 0)
                return;

            await _unitOfWork.UserProjects.CreateAsync(new UserProject
            {
                ProjectId = projectId,
                UserId = userId
            });
        }

        public async Task<PagedResultDto<ProjectDto>> GetUserProjectsAsync(int userId, int pageNumber, int pageSize)
        {
            var userProjects = await _unitOfWork.UserProjects.GetAllAsync(x => x.UserId == userId);
            var projectIds = userProjects.Select(x => x.ProjectId).ToList();

            var projects = await _unitOfWork.Projects.GetPagedAsync(
                x => projectIds.Contains(x.Id),
                pageNumber,
                pageSize);

            var result = projects.ToPagedDto(x => x.ToDto());

            await AttachAssignedUsersAsync(result.Items);

            return result;
        }

        private async Task<IReadOnlyList<Sprint>> GetProjectSprintsAsync(IReadOnlyList<Project> projects)
        {
            var projectIds = projects.Select(x => x.Id).ToList();

            return projectIds.Count == 0
                ? new List<Sprint>()
                : await _unitOfWork.Sprints.GetAllAsync(x => projectIds.Contains(x.ProjectId));
        }

        private async Task<IReadOnlyList<WorkItem>> GetSprintWorkItemsAsync(IReadOnlyList<Sprint> sprints)
        {
            var sprintIds = sprints.Select(x => x.Id).ToList();

            return sprintIds.Count == 0
                ? new List<WorkItem>()
                : await _unitOfWork.WorkItems.GetAllAsync(x => sprintIds.Contains(x.SprintId));
        }

        private async Task AttachAssignedUsersAsync(IReadOnlyList<ProjectDto> projects)
        {
            var projectIds = projects.Select(x => x.Id).ToList();

            if (projectIds.Count == 0)
                return;

            var userProjects = await _unitOfWork.UserProjects.GetAllAsync(x => projectIds.Contains(x.ProjectId));
            var userIds = userProjects.Select(x => x.UserId).Distinct().ToList();

            if (userIds.Count == 0)
                return;

            var users = await _unitOfWork.Users.GetAllAsync(x => userIds.Contains(x.Id));
            var usersById = users.ToDictionary(x => x.Id);
            var assignedUsersByProject = userProjects
                .GroupBy(x => x.ProjectId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(userProject => usersById.TryGetValue(userProject.UserId, out var user)
                            ? ToAssignedUserDto(user)
                            : null)
                        .Where(user => user != null)
                        .Select(user => user!)
                        .OrderBy(user => user.FirstName)
                        .ThenBy(user => user.LastName)
                        .ToList());

            foreach (var project in projects)
            {
                project.AssignedUsers = assignedUsersByProject.TryGetValue(project.Id, out var assignedUsers)
                    ? assignedUsers
                    : new List<ProjectAssignedUserDto>();
            }
        }

        private static ProjectAssignedUserDto ToAssignedUserDto(User user)
        {
            return new ProjectAssignedUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.UserRole,
                IsActive = user.IsActive
            };
        }

        private static void EnsureAssignableUser(User? user)
        {
            if (user == null)
                throw new BadRequestException("User not found.");

            if (user.UserRole != UserRole.User)
                throw new BadRequestException("Only regular users can be assigned.");

            if (!user.IsActive)
                throw new BadRequestException("Cannot assign a deactivated user.");
        }

        private static string NormalizeSearch(string? search)
        {
            return search?.Trim() ?? string.Empty;
        }
    }
}
