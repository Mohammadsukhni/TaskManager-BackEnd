using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;
using TaskManager.Core.Enum;

namespace TaskManager.Core.IService
{
    public interface IWorkItemServices
    {
        Task CreateWorkItemAsync(WorkItemDto dto);

        Task<IReadOnlyList<WorkItemDto>> GetAllWorkItemsAsync();

        Task<WorkItemDto?> GetWorkItemByIdAsync(int id);

        Task UpdateWorkItemAsync(WorkItemDto dto);

        Task DeleteWorkItemAsync(int id);

        Task AssignWorkItemToUserAsync(int workItemId, int userId);

        Task UpdateAssignedWorkItemStatusAsync(int userId, int workItemId, Status status);

        Task UpdateAssignedWorkItemAsync(int userId, WorkItemDto dto);

        Task AddWorkItemRelationAsync(int parentWorkItemId, int childWorkItemId);

        Task<IReadOnlyList<WorkItemDto>> GetUserWorkItemsAsync(int userId);

    }
}
