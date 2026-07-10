using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<SupportMessage>> GetUserConversationAsync(int userId);

        Task<SupportMessage> SendFromUserAsync(int userId, string body);

        Task<SupportMessage> SendFromAdminAsync(int adminId, int targetUserId, string body);

        Task MarkUserMessagesAsReadAsync(int userId);

        Task MarkAdminMessagesAsReadAsync(int userId);

        Task<int> GetUserUnreadCountAsync(int userId);

        Task<int> GetAdminTotalUnreadCountAsync();

        Task<List<ConversationListItemDto>> GetAdminConversationsAsync(string? search = null);

        Task<ConversationViewModel?> GetAdminConversationAsync(int userId);

        Task<bool> UserExistsAsync(int userId);
    }
}
