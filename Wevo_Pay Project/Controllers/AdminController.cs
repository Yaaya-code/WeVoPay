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
        public async Task<IActionResult> PendingTransfers(
    string? search,
    int page = 1)
        {
            int pageSize = 10;


            var result = await _transferService.GetPendingTransfersAsync(
                search,
                page,
                pageSize
            );


            ViewBag.Search = search;

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize
                );


            return View(result.Transfers);
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


            return RedirectToAction(nameof(CompletedTransfers));
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
        public async Task<IActionResult> VerifiedTransfers(
    string? search,
    int page = 1)
        {
            int pageSize = 10;


            var result = await _transferService.GetVerifiedTransfersAsync(
                search,
                page,
                pageSize
            );


            ViewBag.Search = search;

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize
                );


            return View(result.Transfers);
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


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectedTransfers(
                                                        string? search,
                                                        int page = 1)
        {
            int pageSize = 10;


            var result = await _transferService.GetRejectedTransfersAsync(
                search,
                page,
                pageSize
            );


            ViewBag.Search = search;

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize
                );


            return View(result.Transfers);
        }




        public async Task<IActionResult> CompletedTransfers(
            string? search,
            int page = 1)
        {
            int pageSize = 10;


            var result = await _transferService.GetCompletedTransfersAsync(
                search,
                page,
                pageSize
            );


            ViewBag.Search = search;

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize
                );


            return View(result.Transfers);
        }

        public async Task<IActionResult> TransferDetails(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);

            if (transfer == null)
                return NotFound();

            return View(transfer);
        }

       

    }
}