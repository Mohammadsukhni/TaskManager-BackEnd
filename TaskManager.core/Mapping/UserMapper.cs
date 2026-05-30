using TaskManager.Core.Dto;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Mapping
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                UserRole = user.UserRole,
                IsActive = user.IsActive
            };
        }

        public static User ToEntity(this UserDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new User
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = dto.Password,
                UserRole = dto.UserRole,
                IsActive = dto.IsActive
            };
        }
    }
}
