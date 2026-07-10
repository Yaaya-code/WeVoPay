using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Dashboard()
        {
            int userId = GetUserId();
            var model = await _userService.GetDashboardAsync(userId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int userId = GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            var dashboard = await _userService.GetDashboardAsync(userId);

            var model = new UserProfileViewModel
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                IsVerified = user.IsVerified,
                TotalTransfers = dashboard.TotalTransfers,
                CompletedTransfers = dashboard.CompletedTransfers,
                PendingTransfers = dashboard.PendingTransfers
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            int userId = GetUserId();
            var model = await BuildSettingsViewModelAsync(userId);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind(Prefix = "Profile")] UpdateProfileDto dto)
        {
            int userId = GetUserId();

            if (!ModelState.IsValid)
            {
                var invalidModel = await BuildSettingsViewModelAsync(userId, dto);
                return View("Settings", invalidModel);
            }

            try
            {
                await _userService.UpdateProfileAsync(userId, dto);

                await RefreshAuthCookieAsync(userId);

                TempData["SettingsSuccess"] = "Profile updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["SettingsError"] = ex.Message;
            }

            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "Password")] ChangePasswordDto dto)
        {
            int userId = GetUserId();

            if (!ModelState.IsValid)
            {
                var invalidModel = await BuildSettingsViewModelAsync(userId, password: dto);
                return View("Settings", invalidModel);
            }

            try
            {
                await _userService.ChangePasswordAsync(userId, dto);
                TempData["SettingsSuccess"] = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                TempData["SettingsError"] = ex.Message;
            }

            return RedirectToAction(nameof(Settings));
        }

        private async Task<UserSettingsViewModel?> BuildSettingsViewModelAsync(
            int userId,
            UpdateProfileDto? profile = null,
            ChangePasswordDto? password = null)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            return new UserSettingsViewModel
            {
                UserName = user.UserName,
                CreatedAt = user.CreatedAt,
                IsVerified = user.IsVerified,
                Profile = profile ?? new UpdateProfileDto
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                },
                Password = password ?? new ChangePasswordDto()
            };
        }

        private async Task RefreshAuthCookieAsync(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
