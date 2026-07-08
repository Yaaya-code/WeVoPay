using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITransferService _transferService;
        private readonly IAdminService _adminService;
        private readonly ICompanyWalletService _walletService;
        private readonly ISystemSettingService _systemSettingService;

        public AdminController(
            ITransferService transferService,
            IAdminService adminService,
            ICompanyWalletService walletService,
            ISystemSettingService systemSettingService)
        {
            _transferService = transferService;
            _adminService = adminService;
            _walletService = walletService;
            _systemSettingService = systemSettingService;
        }


        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var model = await _adminService.GetDashboardAsync();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> PendingTransfers()
        {
            var transfers = await _transferService.GetPendingTransfersAsync();

            return View(transfers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            var adminId = GetAdminId();


            var result = await _transferService.VerifyTransferAsync(id, adminId);


            if (!result)
                return NotFound();


            return RedirectToAction(nameof(PendingTransfers));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var adminId = GetAdminId();


            var result = await _transferService.CompleteTransferAsync(id, adminId);


            if (!result)
                return NotFound();


            return RedirectToAction(nameof(PendingTransfers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var adminId = GetAdminId();


            var result = await _transferService.RejectTransferAsync(id, adminId);


            if (!result)
                return NotFound();


            return RedirectToAction(nameof(PendingTransfers));
        }

        private int GetAdminId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );
        }

        [HttpGet]
        public async Task<IActionResult> VerifiedTransfers()
        {
            var transfers = await _transferService.GetVerifiedTransfersAsync();

            return View(transfers);
        }

      
        

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var setting = await _systemSettingService.GetAsync();

            if (setting == null)
                return NotFound();

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SystemSetting setting)
        {
            if (!ModelState.IsValid)
                return View(setting);

            var result = await _systemSettingService.UpdateAsync(setting);

            if (!result)
            {
                ModelState.AddModelError("", "Please check the fee percentage and transfer limits.");
                return View(setting);
            }

            return RedirectToAction(nameof(Dashboard));
        }

    }
}