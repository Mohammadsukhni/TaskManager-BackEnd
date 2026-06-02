using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class SprintBackgroundJobService : ISprintBackgroundJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public SprintBackgroundJobService(
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task CloseEndedSprintsAsync()
        {
            var yesterday = TimeZoneInfo
                .ConvertTimeFromUtc(DateTime.UtcNow, GetJordanTimeZone())
                .Date
                .AddDays(-1);

            var endedSprints = await _unitOfWork.Sprints.GetAllAsync(x => x.DateTo.Date == yesterday);

            foreach (var sprint in endedSprints)
            {
                await CloseSprintWorkItemsAsync(sprint);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task CloseSprintWorkItemsAsync(Sprint sprint)
        {
            var workItems = await _unitOfWork.WorkItems.GetAllAsync(x => x.SprintId == sprint.Id);
            var usersById = await GetAssignedUsersByIdAsync(workItems);

            foreach (var workItem in workItems)
            {
                workItem.Status = Status.Done;

                await _unitOfWork.WorkItems.Update(workItem);
                await NotifyAssignedUserAsync(sprint, workItem, usersById);
            }
        }

        private async Task<IReadOnlyDictionary<int, User>> GetAssignedUsersByIdAsync(
            IReadOnlyList<WorkItem> workItems)
        {
            var userIds = workItems
                .Where(x => x.AssignedToUserId.HasValue)
                .Select(x => x.AssignedToUserId!.Value)
                .Distinct()
                .ToList();

            if (userIds.Count == 0)
                return new Dictionary<int, User>();

            var users = await _unitOfWork.Users.GetAllAsync(x => userIds.Contains(x.Id));

            return users.ToDictionary(x => x.Id);
        }

        private async Task NotifyAssignedUserAsync(
            Sprint sprint,
            WorkItem workItem,
            IReadOnlyDictionary<int, User> usersById)
        {
            if (!workItem.AssignedToUserId.HasValue)
                return;

            if (!usersById.TryGetValue(workItem.AssignedToUserId.Value, out var user))
                return;

            var body = $"""
                <h3>Sprint {sprint.Name} has ended.</h3>
                <p>Your task '{workItem.Title}' was closed.</p>
                """;

            await _emailService.SendEmailAsync(user.Email, "Sprint Ended", body);
        }

        private static TimeZoneInfo GetJordanTimeZone()
        {
            foreach (var timeZoneId in new[] { "Jordan Standard Time", "Asia/Amman" })
            {
                if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZone))
                    return timeZone;
            }

            return TimeZoneInfo.Local;
        }
    }
}
