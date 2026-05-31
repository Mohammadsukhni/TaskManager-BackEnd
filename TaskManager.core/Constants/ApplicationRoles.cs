using TaskManager.Core.Enum;

namespace TaskManager.Core.Constants
{
    public static class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static string FromUserRole(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => Admin,
                UserRole.User => User,
                _ => role.ToString()
            };
        }
    }
}
