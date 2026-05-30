using TaskManager.Core.Enum;

namespace TaskManager.Core.Helper
{
    public static class ReferenceNumberHelper
    {
        private static readonly Random _random = new();

        public static string GenerateProjectReference()
        {
            return $"PRJ-{GetTimeStamp()}-{GetRandomNumber()}";
        }

        public static string GenerateSprintReference()
        {
            return $"SPR-{GetTimeStamp()}-{GetRandomNumber()}";
        }

        public static string GenerateWorkItemReference(WorkItemType workItemType)
        {
            string prefix = workItemType switch
            {
                WorkItemType.Feature => "F",
                WorkItemType.UserStory => "US",
                WorkItemType.Task => "T",
                _ => "WI"
            };

            return $"{prefix}-{GetTimeStamp()}-{GetRandomNumber()}";
        }

        private static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private static int GetRandomNumber()
        {
            return _random.Next(100000, 999999);
        }
    }
}