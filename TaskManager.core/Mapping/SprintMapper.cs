using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Mapping
{
    public static class SprintMapper
    {
        public static SprintDto ToDto(this Sprint sprint)
        {
            ArgumentNullException.ThrowIfNull(sprint);

            return new SprintDto
            {
                Id = sprint.Id,
                Name = sprint.Name,
                DateFrom = sprint.DateFrom,
                DateTo = sprint.DateTo,
                ProjectId = sprint.ProjectId,
                ReferenceNumber = sprint.ReferenceNumber,
                CreatedById = sprint.CreatedById,
                LastUpdatedById = sprint.LastUpdatedById
            };
        }

        public static Sprint ToEntity(this SprintDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new Sprint
            {
                Id = dto.Id,
                Name = dto.Name,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                ProjectId = dto.ProjectId
            };
        }
    }
}
