using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface ISprintServices
    {
        Task CreateSprintAsync(SprintDto dto);
        Task<PagedResultDto<SprintDto>> GetAllSprintsAsync(int pageNumber, int pageSize);
        Task<SprintFilterResultDto> FilterSprintsAsync(string? search, int pageNumber, int pageSize);
        Task<SprintDto?> GetSprintByIdAsync(int id);
        Task UpdateSprintAsync(SprintDto dto);
        Task DeleteSprintAsync(int id);
        Task<PagedResultDto<SprintDto>> GetUserSprintsAsync(int userId, int pageNumber, int pageSize);
    }
}
