using TaskManager.Core.Enum;

namespace TaskManager.Core.Entities
{
    public class Otp : BaseEntity
    {
        public int ReceiverId { get; set; }
        public string Code { get; set; } = string.Empty;
        public OtpActionType ActionType { get; set; }
        public User Receiver { get; set; } = null!;
    }
}
