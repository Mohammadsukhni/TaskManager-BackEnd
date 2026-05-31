using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Dto
{
    public class ProjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ReferenceNumber { get; set; } = string.Empty;

        public int? CreatedById { get; set; }

        public int? LastUpdatedById { get; set; }
    }
}
