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

            var sprints = await _unitOfWork.Sprints.GetAllAsync();

            var endedSprints = sprints
                .Where(x => x.DateTo.Date == yesterday)
                .ToList();

            foreach (var sprint in endedSprints)
            {
                var workItems = await _unitOfWork.WorkItems.GetAllAsync();

                var sprintWorkItems = workItems
                    .Where(x => x.SprintId == sprint.Id)
                    .ToList();

                foreach (var workItem in sprintWorkItems)
                {
                    workItem.Status = Status.Done;

                    _unitOfWork.WorkItems.Update(workItem);

                    if (workItem.AssignedToUserId != null)
                    {
                        var user = await _unitOfWork.Users.GetByIdAsync(workItem.AssignedToUserId.Value);

                        if (user != null)
                        {
                            await _emailService.SendEmailAsync(user.Email, "Sprint Ended", $"<h3>Sprint {sprint.Name} has ended.</h3><p>Your task '{workItem.Title}' was closed.</p>");
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
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
