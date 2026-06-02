using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface IUserService
    {
        Task<PagedResultDto<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<UserFilterResultDto> FilterUsersAsync(string? search, int pageNumber, int pageSize);

        Task<UserDto?> GetUserByIdAsync(int id);

        Task CreateUserAsync(UserDto dto);

        Task UpdateUserAsync(UserDto dto);

        Task UpdateOwnAccountAsync(int userId, UserDto dto);
        Task ChangeUserStatusAsync(int userId, bool isActive);

        Task DeleteUserAsync(int id);
    }
}
