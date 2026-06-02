using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Exceptions;
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
                throw new BadRequestException("Sprint not found.");

            if (IsSprintEnded(sprint.DateTo))
                throw new BadRequestException("Cannot add work item to an ended sprint.");

            dto.EstimatedTime = ToWholeHours(dto.EstimatedTime);
            dto.ActualTime = TimeSpan.Zero;
            dto.Status = Status.New;

            var workItem = dto.ToEntity();

            workItem.ReferenceNumber =
                ReferenceNumberHelper.GenerateWorkItemReference(dto.Type);

            await _unitOfWork.WorkItems.CreateAsync(workItem);
        }

        public async Task<PagedResultDto<WorkItemDto>> GetAllWorkItemsAsync(
            int pageNumber,
            int pageSize)
        {
            var workItems = await _unitOfWork.WorkItems.GetPagedAsync(pageNumber, pageSize);

            var result = workItems.ToPagedDto(x => x.ToDto());
            await AttachChildrenAsync(result.Items);

            return result;
        }

        public async Task<PagedResultDto<WorkItemDto>> FilterWorkItemsAsync(
            string? search,
            int pageNumber,
            int pageSize)
        {
            var term = NormalizeSearch(search);

            if (string.IsNullOrWhiteSpace(term))
                return await GetAllWorkItemsAsync(pageNumber, pageSize);

            var hasStatus = Enum.TryParse<Status>(term, true, out var status);
            var hasType = Enum.TryParse<WorkItemType>(term, true, out var type);
            var hasTime = TimeSpan.TryParse(term, out var time);

            var workItems = await _unitOfWork.WorkItems.GetPagedAsync(
                x => x.Title.Contains(term) ||
                     x.Description.Contains(term) ||
                     x.ReferenceNumber.Contains(term) ||
                     (hasStatus && x.Status == status) ||
                     (hasType && x.Type == type) ||
                     (hasTime && (x.EstimatedTime == time ||
                                  x.ActualTime == time)),
                pageNumber,
                pageSize);

            var result = workItems.ToPagedDto(x => x.ToDto());
            await AttachChildrenAsync(result.Items);

            return result;
        }

        public async Task<WorkItemDto?> GetWorkItemByIdAsync(int id)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(id);

            if (workItem == null)
                return null;

            var dto = workItem.ToDto();
            await AttachChildrenAsync(new[] { dto });

            return dto;
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
                throw new BadRequestException("Cannot move work item to an ended sprint.");

            var relations = await _unitOfWork.WorkItemRelations.GetAllAsync(
                x => x.ParentWorkItemId == workItem.Id ||
                     x.ChildWorkItemId == workItem.Id);
            var hasRelations = relations.Count > 0;

            if (hasRelations && (workItem.Type != dto.Type || workItem.SprintId != dto.SprintId))
                return;

            workItem.Title = dto.Title;
            workItem.Description = dto.Description;
            workItem.EstimatedTime = ToWholeHours(dto.EstimatedTime);
            workItem.Status = dto.Status;
            workItem.Type = dto.Type;
            workItem.SprintId = dto.SprintId;

            await _unitOfWork.WorkItems.Update(workItem);
        }

        public async Task DeleteWorkItemAsync(int id)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(id);

            if (workItem == null)
                return;

            await _unitOfWork.WorkItems.Delete(workItem);
        }

        public async Task AssignWorkItemToUserAsync(int workItemId, int userId)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(workItemId);

            if (workItem == null)
                throw new BadRequestException("Work item not found.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            EnsureAssignableUser(user);

            var sprint = await _unitOfWork.Sprints.GetByIdAsync(workItem.SprintId);

            if (sprint == null)
                throw new BadRequestException("Sprint not found.");

            var projectAssignments = await _unitOfWork.UserProjects.GetAllAsync(
                x => x.UserId == userId &&
                     x.ProjectId == sprint.ProjectId);

            if (projectAssignments.Count == 0)
                throw new BadRequestException("User must be assigned to the project first.");

            workItem.AssignedToUserId = userId;

            await _unitOfWork.WorkItems.Update(workItem);
            await SendAssignmentEmailAsync(user!, workItem);
        }

        public async Task AddWorkItemRelationAsync(int parentWorkItemId, int childWorkItemId)
        {
            if (parentWorkItemId == childWorkItemId)
                throw new BadRequestException("Parent and child work items must be different.");

            var parent = await _unitOfWork.WorkItems.GetByIdAsync(parentWorkItemId);
            var child = await _unitOfWork.WorkItems.GetByIdAsync(childWorkItemId);

            if (parent == null || child == null)
                throw new BadRequestException("Work item not found.");

            if (parent.SprintId != child.SprintId)
                throw new BadRequestException("Parent and child must be in the same sprint.");

            var sprint = await _unitOfWork.Sprints.GetByIdAsync(parent.SprintId);

            if (sprint == null)
                throw new BadRequestException("Sprint not found.");

            if (!IsValidRelation(parent.Type, child.Type))
                throw new BadRequestException(GetRelationRuleMessage(parent.Type));

            var existingRelations = await _unitOfWork.WorkItemRelations.GetAllAsync(
                x => x.ParentWorkItemId == parentWorkItemId &&
                     x.ChildWorkItemId == childWorkItemId);

            if (existingRelations.Count > 0)
                throw new BadRequestException("Work item relation already exists.");

            await _unitOfWork.WorkItemRelations.CreateAsync(new WorkItemRelation
            {
                ParentWorkItemId = parentWorkItemId,
                ChildWorkItemId = childWorkItemId
            });
        }

        public async Task<PagedResultDto<WorkItemDto>> GetUserWorkItemsAsync(
            int userId,
            int pageNumber,
            int pageSize)
        {
            var workItems = await _unitOfWork.WorkItems.GetPagedAsync(
                x => x.AssignedToUserId == userId,
                pageNumber,
                pageSize);

            var result = workItems.ToPagedDto(x => x.ToDto());
            await AttachChildrenAsync(result.Items);

            return result;
        }

        public async Task UpdateAssignedWorkItemStatusAsync(int userId, int workItemId, Status status)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(workItemId);

            if (workItem == null)
                return;

            if (workItem.AssignedToUserId != userId)
                return;

            workItem.Status = status;

            await _unitOfWork.WorkItems.Update(workItem);
        }

        public async Task UpdateAssignedWorkItemAsync(int userId, WorkItemDto dto)
        {
            var workItem = await _unitOfWork.WorkItems.GetByIdAsync(dto.Id);

            if (workItem == null)
                return;

            if (workItem.AssignedToUserId != userId)
                return;

            workItem.ActualTime = ToWholeHours(dto.ActualTime);

            await _unitOfWork.WorkItems.Update(workItem);
        }

        private async Task AttachChildrenAsync(IEnumerable<WorkItemDto> workItems)
        {
            var itemList = workItems.ToList();

            if (itemList.Count == 0)
                return;

            var parentIds = itemList.Select(x => x.Id).ToList();
            var relations = await _unitOfWork.WorkItemRelations.GetAllAsync(
                x => parentIds.Contains(x.ParentWorkItemId));

            if (relations.Count == 0)
                return;

            var childIds = relations.Select(x => x.ChildWorkItemId).Distinct().ToList();
            var children = await _unitOfWork.WorkItems.GetAllAsync(x => childIds.Contains(x.Id));
            var childrenById = children.ToDictionary(x => x.Id);
            var relationsByParent = relations
                .GroupBy(x => x.ParentWorkItemId)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var item in itemList)
            {
                if (!relationsByParent.TryGetValue(item.Id, out var itemRelations))
                    continue;

                item.Children = itemRelations
                    .Select(x => childrenById.TryGetValue(x.ChildWorkItemId, out var child) ? child : null)
                    .Where(child => child != null)
                    .Select(child => ToChildDto(child!))
                    .ToList();
            }
        }

        private async Task SendAssignmentEmailAsync(User user, WorkItem workItem)
        {
            var body = $"""
                <h3>New Work Item Assigned To You</h3>
                <p>Title: {workItem.Title}</p>
                <p>Description: {workItem.Description}</p>
                <p>Status: {workItem.Status}</p>
                """;

            await _emailService.SendEmailAsync(user.Email, "New Work Item Assigned", body);
        }

        private static WorkItemChildDto ToChildDto(WorkItem child)
        {
            return new WorkItemChildDto
            {
                Id = child.Id,
                Title = child.Title,
                Status = child.Status,
                Type = child.Type,
                ReferenceNumber = child.ReferenceNumber
            };
        }

        private static void EnsureAssignableUser(User? user)
        {
            if (user == null)
                throw new BadRequestException("User not found.");

            if (user.UserRole != UserRole.User)
                throw new BadRequestException("Only regular users can be assigned.");

            if (!user.IsActive)
                throw new BadRequestException("Cannot assign a deactivated user.");
        }

        private static bool IsSprintEnded(DateTime dateTo)
        {
            return dateTo.Date < DateTime.UtcNow.Date;
        }

        private static string NormalizeSearch(string? search)
        {
            return search?.Trim() ?? string.Empty;
        }

        private static TimeSpan ToWholeHours(TimeSpan time)
        {
            var hours = Math.Max(0, (int)Math.Floor(time.TotalHours));

            return TimeSpan.FromHours(hours);
        }

        private static bool IsValidRelation(WorkItemType parentType, WorkItemType childType)
        {
            return parentType switch
            {
                WorkItemType.Feature => childType == WorkItemType.UserStory,
                WorkItemType.UserStory => childType == WorkItemType.Task,
                _ => false
            };
        }

        private static string GetRelationRuleMessage(WorkItemType parentType)
        {
            return parentType switch
            {
                WorkItemType.Feature => "Feature can only contain User Story work items.",
                WorkItemType.UserStory => "User Story can only contain Task work items.",
                WorkItemType.Task => "Task cannot contain child work items.",
                _ => "Invalid work item relation."
            };
        }
    }
}
