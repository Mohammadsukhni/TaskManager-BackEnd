using TaskManager.Core.Dto;
using TaskManager.Core.Enum;
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
            PasswordPolicyHelper.Validate(dto.Password);

            var user = dto.ToEntity();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IsActive = user.UserRole == UserRole.Admin || dto.IsActive;

            await _unitOfWork.Users.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _emailService.SendEmailAsync(user.Email, "Task Manager Account", $@" <h3>Welcome To Task Manager</h3> <p>Email: {user.Email}</p> <p>Password: {dto.Password}</p>");
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return;

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return users.Select(x => x.ToDto()).ToList();
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
            user.Phone = dto.Phone;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);

            if (user == null)
                return;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.UserRole = dto.UserRole;
            if (user.UserRole == UserRole.Admin)
            {
                user.IsActive = true;
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ChangeUserStatusAsync(int userId, bool isActive)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return;

            if (user.UserRole == UserRole.Admin)
            {
                user.IsActive = true;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                if (!isActive)
                    throw new InvalidOperationException("Admin is always active.");

                return;
            }

            user.IsActive = isActive;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
