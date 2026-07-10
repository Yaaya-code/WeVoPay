using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.DTOs
{
    public class ConversationViewModel
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<SupportMessage> Messages { get; set; } = new();
        public SendMessageDto Reply { get; set; } = new();
        public int UnreadCount { get; set; }
    }
}
