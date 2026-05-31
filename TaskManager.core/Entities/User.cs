using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;

namespace TaskManager.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public ICollection<Otp> Otps { get; set; } = new List<Otp>();
        public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();

    }
}
