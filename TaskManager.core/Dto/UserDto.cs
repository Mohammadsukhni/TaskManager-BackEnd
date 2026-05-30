using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Enum;

namespace TaskManager.Core.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
