using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SystemSettingController : Controller
    {
        private readonly ISystemSettingService _systemSettingService;

        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var setting = await _systemSettingService.GetAsync();

            if (setting == null)
                return NotFound();

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SystemSetting setting)
        {
            if (!ModelState.IsValid)
                return View(setting);

            var result = await _systemSettingService.UpdateAsync(setting);

            if (!result)
            {
                ModelState.AddModelError("", "Invalid settings.");
                return View(setting);
            }

            TempData["Success"] = "Settings updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
