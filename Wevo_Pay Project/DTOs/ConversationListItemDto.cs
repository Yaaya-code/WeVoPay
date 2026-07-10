namespace Wevo_Pay_Project.DTOs
{
    public class ConversationListItemDto
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastMessagePreview { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        public bool LastMessageFromAdmin { get; set; }
    }
}
