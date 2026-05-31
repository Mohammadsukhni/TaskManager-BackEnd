using TaskManager.Core.Exceptions;

namespace TaskManager.Core.Helper
{
    public static class PasswordPolicyHelper
    {
        public static void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                password.Length < 8 ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) ||
                !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                throw new BadRequestException(
                    "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character");
            }
        }
    }
}
