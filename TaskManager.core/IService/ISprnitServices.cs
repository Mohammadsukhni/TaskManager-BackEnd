using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface ISprintServices
    {
        Task CreateSprintAsync(SprintDto dto);
        Task<IReadOnlyList<SprintDto>> GetAllSprintsAsync();
        Task<SprintDto?> GetSprintByIdAsync(int id);
        Task UpdateSprintAsync(SprintDto dto);
        Task DeleteSprintAsync(int id);
        Task<IReadOnlyList<SprintDto>> GetUserSprintsAsync(int userId);
    }
}
