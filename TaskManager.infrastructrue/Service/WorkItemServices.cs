using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Helper;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Core.Mapping;

namespace TaskManager.Infrastructure.Service
{
    public class WorkItemServices : IWorkItemServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        public WorkItemServices(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task CreateWorkItemAsync(WorkItemDto dto)
        {
            var sprint = await _unitOfWork.Sprints.GetByIdAsync(dto.SprintId);

            if (sprint == null)
                throw new InvalidOperationException("Sprint not found.");

            if (IsSprintEnded(sprint.DateTo))
                throw new InvalidOperationException("Cannot add work item to an ended sprint.");

            var workItem = dto.ToEntity();

            workItem.ReferenceNumber =
                ReferenceNumberHelper.GenerateWorkItemReference(dto.Type);

            await _unitOfWork.WorkItems.CreateAsync(workItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<WorkItemDto>> GetAllWorkItemsAsync()
        {
            var workItems = await _unitOfWork.WorkItems.GetAllAsync();

            return workItems.Select(x => x.ToDto()).ToList();
        }

        public async Task<WorkItemDto?> GetWorkItemByIdAsync(int id)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(id);

            if (workItem == null)
                return null;

            return workItem.ToDto();
        }

        public async Task UpdateWorkItemAsync(WorkItemDto dto)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(dto.Id);

            if (workItem == null)
                return;

            var sprint = await _unitOfWork.Sprints.GetByIdAsync(dto.SprintId);

            if (sprint == null)
                return;

            if (workItem.SprintId != dto.SprintId && IsSprintEnded(sprint.DateTo))
                throw new InvalidOperationException("Cannot move work item to an ended sprint.");

            var hasRelations = (await _unitOfWork.WorkItemRelations.GetAllAsync())
                .Any(x => x.ParentWorkItemId == workItem.Id ||
                          x.ChildWorkItemId == workItem.Id);

            if (hasRelations && (workItem.Type != dto.Type || workItem.SprintId != dto.SprintId))
                return;

            workItem.Title = dto.Title;
            workItem.Description = dto.Description;
            workItem.EstimatedTime = dto.EstimatedTime;
            workItem.ActualTime = dto.ActualTime;
            workItem.Status = dto.Status;
            workItem.Type = dto.Type;
            workItem.SprintId = dto.SprintId;

            _unitOfWork.WorkItems.Update(workItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteWorkItemAsync(int id)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(id);

            if (workItem == null)
                return;

            _unitOfWork.WorkItems.Delete(workItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignWorkItemToUserAsync(int workItemId, int userId)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(workItemId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (workItem == null)
                throw new InvalidOperationException("Work item not found.");

            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (user.UserRole != UserRole.User)
                throw new InvalidOperationException("Only regular users can be assigned.");

            if (!user.IsActive)
                throw new InvalidOperationException("Cannot assign a deactivated user.");

            var sprint = await _unitOfWork.Sprints.GetByIdAsync(workItem.SprintId);

            if (sprint == null)
                throw new InvalidOperationException("Sprint not found.");

            var isUserInProject = (await _unitOfWork.UserProjects.GetAllAsync())
                .Any(x => x.UserId == userId &&
                          x.ProjectId == sprint.ProjectId);

            if (!isUserInProject)
                throw new InvalidOperationException("User must be assigned to the project first.");

            workItem.AssignedToUserId = userId;

            _unitOfWork.WorkItems.Update(workItem);
            await _unitOfWork.SaveChangesAsync();
            await _emailService.SendEmailAsync( user.Email, "New Work Item Assigned", $@" <h3>New Work Item Assigned To You</h3> <p>Title: {workItem.Title}</p> <p>Description: {workItem.Description}</p> <p>Status: {workItem.Status}</p>");
        }

        public async Task AddWorkItemRelationAsync(int parentWorkItemId, int childWorkItemId)
        {
            if (parentWorkItemId == childWorkItemId)
                return;

            var parent = await _unitOfWork.WorkItems.GetByIdAsync(parentWorkItemId);
            var child = await _unitOfWork.WorkItems.GetByIdAsync(childWorkItemId);

            if (parent == null || child == null)
                return;

            if (parent.SprintId != child.SprintId)
                return;

            var parentSprint = await _unitOfWork.Sprints.GetByIdAsync(parent.SprintId);
            var childSprint = await _unitOfWork.Sprints.GetByIdAsync(child.SprintId);

            if (parentSprint == null || childSprint == null)
                return;

            if (parentSprint.ProjectId != childSprint.ProjectId)
                return;

            if (parent.Type == WorkItemType.Task)
                return;

            if (parent.Type == WorkItemType.Feature &&
                child.Type != WorkItemType.UserStory)
                return;

            if (parent.Type == WorkItemType.UserStory &&
                child.Type != WorkItemType.Task)
                return;

            var exists = (await _unitOfWork.WorkItemRelations.GetAllAsync())
                .Any(x => x.ParentWorkItemId == parentWorkItemId &&
                          x.ChildWorkItemId == childWorkItemId);

            if (exists)
                return;

            var relation = new WorkItemRelation
            {
                ParentWorkItemId = parentWorkItemId,
                ChildWorkItemId = childWorkItemId
            };

            await _unitOfWork.WorkItemRelations.CreateAsync(relation);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<WorkItemDto>> GetUserWorkItemsAsync(int userId)
        {
            var workItems = await _unitOfWork.WorkItems.GetAllAsync();

            return workItems
                .Where(x => x.AssignedToUserId == userId)
                .Select(x => x.ToDto())
                .ToList();
        }

        public async Task UpdateAssignedWorkItemStatusAsync(int userId,int workItemId,Status status)
        {
            var workItem =
                await _unitOfWork.WorkItems.GetByIdAsync(workItemId);

            if (workItem == null)
                return;

            if (workItem.AssignedToUserId != userId)
                return;

            workItem.Status = status;

            _unitOfWork.WorkItems.Update(workItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAssignedWorkItemAsync(int userId, WorkItemDto dto)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(dto.Id);

            if (workItem == null)
                return;

            if (workItem.AssignedToUserId != userId)
                return;

            workItem.Title = dto.Title;
            workItem.Description = dto.Description;
            workItem.ActualTime = dto.ActualTime;
            workItem.Status = dto.Status;

            _unitOfWork.WorkItems.Update(workItem);
            await _unitOfWork.SaveChangesAsync();
        }

        private static bool IsSprintEnded(DateTime dateTo)
        {
            return dateTo.Date < DateTime.UtcNow.Date;
        }
    }
}
