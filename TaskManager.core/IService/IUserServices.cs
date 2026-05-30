using TaskManager.Core.Dto;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync();

    Task<UserDto?> GetUserByIdAsync(int id);

    Task CreateUserAsync(UserDto dto);

    Task UpdateUserAsync(UserDto dto); // Admin

    Task UpdateOwnAccountAsync(int userId, UserDto dto); // User
    Task ChangeUserStatusAsync(int userId, bool isActive);

    Task DeleteUserAsync(int id);
}