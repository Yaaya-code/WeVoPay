using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            var messages = await _messageService.GetUserConversationAsync(userId);
            await _messageService.MarkUserMessagesAsReadAsync(userId);

            var model = new ConversationViewModel
            {
                UserId = userId,
                UserFullName = User.Identity?.Name ?? "You",
                Messages = messages,
                Reply = new SendMessageDto(),
                UnreadCount = 0
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send([Bind(Prefix = "Reply")] SendMessageDto dto)
        {
            int userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                var messages = await _messageService.GetUserConversationAsync(userId);
                var model = new ConversationViewModel
                {
                    UserId = userId,
                    UserFullName = User.Identity?.Name ?? "You",
                    Messages = messages,
                    Reply = dto
                };
                return View("Index", model);
            }

            try
            {
                await _messageService.SendFromUserAsync(userId, dto.Body);
                TempData["MessageSuccess"] = "Message sent to support.";
            }
            catch (Exception ex)
            {
                TempData["MessageError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(string? search = null)
        {
            var conversations = await _messageService.GetAdminConversationsAsync(search);
            ViewBag.Search = search ?? string.Empty;
            ViewBag.TotalUnread = await _messageService.GetAdminTotalUnreadCountAsync();
            return View(conversations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminConversation(int userId)
        {
            var model = await _messageService.GetAdminConversationAsync(userId);

            if (model == null)
                return NotFound();

            await _messageService.MarkAdminMessagesAsReadAsync(userId);
            model.UnreadCount = 0;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminReply([Bind(Prefix = "Reply")] SendMessageDto dto)
        {
            if (dto.TargetUserId == null || dto.TargetUserId <= 0)
            {
                TempData["MessageError"] = "Invalid conversation.";
                return RedirectToAction(nameof(AdminIndex));
            }

            int targetUserId = dto.TargetUserId.Value;
            int adminId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                var model = await _messageService.GetAdminConversationAsync(targetUserId);
                if (model == null)
                    return NotFound();

                model.Reply = dto;
                return View("AdminConversation", model);
            }

            try
            {
                await _messageService.SendFromAdminAsync(adminId, targetUserId, dto.Body);
                TempData["MessageSuccess"] = "Reply sent.";
            }
            catch (Exception ex)
            {
                TempData["MessageError"] = ex.Message;
            }

            return RedirectToAction(nameof(AdminConversation), new { userId = targetUserId });
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
