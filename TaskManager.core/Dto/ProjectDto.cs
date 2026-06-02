using TaskManager.Core.Enum;

namespace TaskManager.Core.Dto
{
    public class ProjectAssignedUserDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole UserRole { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ReferenceNumber { get; set; } = string.Empty;

        public int? CreatedById { get; set; }

        public int? LastUpdatedById { get; set; }

        public IReadOnlyList<ProjectAssignedUserDto> AssignedUsers { get; set; } = new List<ProjectAssignedUserDto>();
    }
}
