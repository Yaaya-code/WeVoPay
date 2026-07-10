using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupportMessage>> GetUserConversationAsync(int userId)
        {
            return await _context.SupportMessages
                .Include(m => m.Sender)
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<SupportMessage> SendFromUserAsync(int userId, string body)
        {
            body = NormalizeBody(body);

            var message = new SupportMessage
            {
                UserId = userId,
                SenderId = userId,
                Body = body,
                IsFromAdmin = false,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportMessages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<SupportMessage> SendFromAdminAsync(int adminId, int targetUserId, string body)
        {
            body = NormalizeBody(body);

            var userExists = await UserExistsAsync(targetUserId);
            if (!userExists)
                throw new Exception("User not found.");

            var message = new SupportMessage
            {
                UserId = targetUserId,
                SenderId = adminId,
                Body = body,
                IsFromAdmin = true,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportMessages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task MarkUserMessagesAsReadAsync(int userId)
        {
            var unread = await _context.SupportMessages
                .Where(m => m.UserId == userId && m.IsFromAdmin && !m.IsRead)
                .ToListAsync();

            if (unread.Count == 0)
                return;

            foreach (var msg in unread)
                msg.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAdminMessagesAsReadAsync(int userId)
        {
            var unread = await _context.SupportMessages
                .Where(m => m.UserId == userId && !m.IsFromAdmin && !m.IsRead)
                .ToListAsync();

            if (unread.Count == 0)
                return;

            foreach (var msg in unread)
                msg.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserUnreadCountAsync(int userId)
        {
            return await _context.SupportMessages
                .CountAsync(m => m.UserId == userId && m.IsFromAdmin && !m.IsRead);
        }

        public async Task<int> GetAdminTotalUnreadCountAsync()
        {
            return await _context.SupportMessages
                .CountAsync(m => !m.IsFromAdmin && !m.IsRead);
        }

        public async Task<List<ConversationListItemDto>> GetAdminConversationsAsync(string? search = null)
        {
            var query = _context.SupportMessages
                .Include(m => m.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(m =>
                    m.User.FullName.Contains(search) ||
                    m.User.UserName.Contains(search) ||
                    m.User.Email.Contains(search) ||
                    m.Body.Contains(search));
            }

            var userIds = await query
                .Where(m => m.User.Role == UserRole.User)
                .Select(m => m.UserId)
                .Distinct()
                .ToListAsync();

            var conversations = new List<ConversationListItemDto>();

            foreach (var userId in userIds)
            {
                var messages = await _context.SupportMessages
                    .Include(m => m.User)
                    .Where(m => m.UserId == userId)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                if (messages.Count == 0)
                    continue;

                var last = messages.First();
                var preview = last.Body.Length > 80
                    ? last.Body.Substring(0, 78) + "..."
                    : last.Body;

                conversations.Add(new ConversationListItemDto
                {
                    UserId = userId,
                    UserFullName = last.User.FullName,
                    UserName = last.User.UserName,
                    Email = last.User.Email,
                    LastMessagePreview = preview,
                    LastMessageAt = last.CreatedAt,
                    LastMessageFromAdmin = last.IsFromAdmin,
                    UnreadCount = messages.Count(m => !m.IsFromAdmin && !m.IsRead)
                });
            }

            return conversations
                .OrderByDescending(c => c.UnreadCount > 0)
                .ThenByDescending(c => c.LastMessageAt)
                .ToList();
        }

        public async Task<ConversationViewModel?> GetAdminConversationAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRole.User);

            if (user == null)
                return null;

            var messages = await GetUserConversationAsync(userId);

            return new ConversationViewModel
            {
                UserId = user.Id,
                UserFullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Messages = messages,
                UnreadCount = messages.Count(m => !m.IsFromAdmin && !m.IsRead),
                Reply = new SendMessageDto { TargetUserId = user.Id }
            };
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == userId && u.Role == UserRole.User);
        }

        private static string NormalizeBody(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                throw new Exception("Message cannot be empty.");

            body = body.Trim();

            if (body.Length > 2000)
                throw new Exception("Message cannot exceed 2000 characters.");

            return body;
        }
    }
}
