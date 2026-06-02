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
    public class UserServices : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public UserServices(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task CreateUserAsync(UserDto dto)
        {
            dto.Phone = NormalizePhone(dto.Phone);
            await EnsurePhoneIsUniqueAsync(dto.Phone);

            PasswordPolicyHelper.Validate(dto.Password);

            var user = dto.ToEntity();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IsActive = user.UserRole == UserRole.Admin || dto.IsActive;

            await _unitOfWork.Users.CreateAsync(user);
            await SendWelcomeEmailAsync(user, dto.Password);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return;

            await _unitOfWork.Users.Delete(user);
        }

        public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _unitOfWork.Users.GetPagedAsync(pageNumber, pageSize);

            return users.ToPagedDto(x => x.ToDto());
        }

        public async Task<UserFilterResultDto> FilterUsersAsync(string? search, int pageNumber, int pageSize)
        {
            var users = await GetFilteredUsersAsync(search, pageNumber, pageSize);
            var userIds = users.Items.Select(x => x.Id).ToList();
            var userProjects = await GetUserProjectsAsync(userIds);
            var projectIds = userProjects.Select(x => x.ProjectId).Distinct().ToList();
            var projects = await GetProjectsAsync(projectIds);
            var sprints = await GetProjectSprintsAsync(projectIds);
            var workItems = await GetRelatedWorkItemsAsync(userIds, sprints);

            return new UserFilterResultDto
            {
                Users = users.ToPagedDto(x => x.ToDto()),
                Projects = projects.Select(x => x.ToDto()).ToList(),
                Sprints = sprints.Select(x => x.ToDto()).ToList(),
                WorkItems = workItems.Select(x => x.ToDto()).ToList()
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return null;

            return user.ToDto();
        }

        public async Task UpdateOwnAccountAsync(int userId, UserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = NormalizePhone(dto.Phone);

            await EnsurePhoneIsUniqueAsync(user.Phone, user.Id);
            await _unitOfWork.Users.Update(user);
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);

            if (user == null)
                return;

            dto.Phone = NormalizePhone(dto.Phone);
            await EnsurePhoneIsUniqueAsync(dto.Phone, user.Id);

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            user.UserRole = dto.UserRole;
            user.IsActive = user.UserRole == UserRole.Admin || dto.IsActive;

            var password = dto.Password?.Trim();

            if (!string.IsNullOrWhiteSpace(password))
            {
                PasswordPolicyHelper.Validate(password);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            await _unitOfWork.Users.Update(user);
        }

        public async Task ChangeUserStatusAsync(int userId, bool isActive)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return;

            if (user.UserRole == UserRole.Admin)
            {
                if (!isActive)
                    throw new BadRequestException("Admin is always active.");

                user.IsActive = true;
                await _unitOfWork.Users.Update(user);
                return;
            }

            user.IsActive = isActive;

            await _unitOfWork.Users.Update(user);
        }

        private async Task<PagedResultDto<User>> GetFilteredUsersAsync(
            string? search,
            int pageNumber,
            int pageSize)
        {
            var term = NormalizeSearch(search);

            if (string.IsNullOrWhiteSpace(term))
                return await _unitOfWork.Users.GetPagedAsync(pageNumber, pageSize);

            var hasRole = Enum.TryParse<UserRole>(term, true, out var role);

            return await _unitOfWork.Users.GetPagedAsync(
                x => x.FirstName.Contains(term) ||
                     x.LastName.Contains(term) ||
                     (x.FirstName + " " + x.LastName).Contains(term) ||
                     x.Email.Contains(term) ||
                     x.Phone.Contains(term) ||
                     (hasRole && x.UserRole == role),
                pageNumber,
                pageSize);
        }

        private async Task<IReadOnlyList<UserProject>> GetUserProjectsAsync(IReadOnlyList<int> userIds)
        {
            return userIds.Count == 0
                ? new List<UserProject>()
                : await _unitOfWork.UserProjects.GetAllAsync(x => userIds.Contains(x.UserId));
        }

        private async Task<IReadOnlyList<Project>> GetProjectsAsync(IReadOnlyList<int> projectIds)
        {
            return projectIds.Count == 0
                ? new List<Project>()
                : await _unitOfWork.Projects.GetAllAsync(x => projectIds.Contains(x.Id));
        }

        private async Task<IReadOnlyList<Sprint>> GetProjectSprintsAsync(IReadOnlyList<int> projectIds)
        {
            return projectIds.Count == 0
                ? new List<Sprint>()
                : await _unitOfWork.Sprints.GetAllAsync(x => projectIds.Contains(x.ProjectId));
        }

        private async Task<IReadOnlyList<WorkItem>> GetRelatedWorkItemsAsync(
            IReadOnlyList<int> userIds,
            IReadOnlyList<Sprint> sprints)
        {
            var sprintIds = sprints.Select(x => x.Id).ToList();

            if (userIds.Count == 0 && sprintIds.Count == 0)
                return new List<WorkItem>();

            return await _unitOfWork.WorkItems.GetAllAsync(x =>
                (x.AssignedToUserId.HasValue && userIds.Contains(x.AssignedToUserId.Value)) ||
                sprintIds.Contains(x.SprintId));
        }

        private async Task SendWelcomeEmailAsync(User user, string password)
        {
            var body = $"""
                <h3>Welcome To Task Manager</h3>
                <p>Email: {user.Email}</p>
                <p>Password: {password}</p>
                """;

            await _emailService.SendEmailAsync(user.Email, "Task Manager Account", body);
        }

        private async Task EnsurePhoneIsUniqueAsync(string phone, int? excludedUserId = null)
        {
            var users = await _unitOfWork.Users.GetAllAsync(x => x.Phone == phone);
            var phoneExists = users.Any(x => !excludedUserId.HasValue || x.Id != excludedUserId.Value);

            if (phoneExists)
                throw new BadRequestException("Phone number already exists.");
        }

        private static string NormalizeSearch(string? search)
        {
            return search?.Trim() ?? string.Empty;
        }

        private static string NormalizePhone(string? phone)
        {
            var normalizedPhone = phone?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedPhone))
                throw new BadRequestException("Phone number is required.");

            return normalizedPhone;
        }
    }
}
