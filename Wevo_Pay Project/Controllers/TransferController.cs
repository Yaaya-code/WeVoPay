using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;
        private readonly ICompanyWalletService _walletService;
        private readonly ISystemSettingService _settingService;

        public TransferController(
            ITransferService transferService,
            ICompanyWalletService walletService,
            ISystemSettingService settingService)
        {
            _transferService = transferService;
            _walletService = walletService;
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCreateFormDataAsync();
            return View(new CreateTransferDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTransferDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateFormDataAsync();
                return View(dto);
            }

            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var transfer = await _transferService.CreateTransferAsync(userId, dto);
                return RedirectToAction("Success", new { id = transfer.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadCreateFormDataAsync();
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyTransfers(
            string search = "",
            TransferStatus? status = null,
            int page = 1)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            const int pageSize = 10;

            var transfers = await _transferService.GetUserTransfersAsync(
                userId, search, status, page, pageSize);

            var totalCount = await _transferService.GetUserTransfersCountAsync(
                userId, search, status);

            ViewBag.Search = search ?? string.Empty;
            ViewBag.Status = status;
            ViewBag.Page = page < 1 ? 1 : page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling((double)totalCount / pageSize));

            return View(transfers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var transfer = await _transferService.GetUserTransferByIdAsync(id, userId);

            if (transfer == null)
                return NotFound();

            return View(transfer);
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var transfer = await _transferService.GetUserTransferByIdAsync(id, userId);

            if (transfer == null)
                return NotFound();

            return View(transfer);
        }

        private async Task LoadCreateFormDataAsync()
        {
            var wallets = await _walletService.GetAllAsync();
            ViewBag.Wallets = wallets.Where(w => w.IsActive).ToList();

            var settings = await _settingService.GetAsync();
            ViewBag.FeePercentage = settings?.FeePercentage ?? 1.5m;
            ViewBag.MinTransferAmount = settings?.MinTransferAmount ?? 10m;
            ViewBag.MaxTransferAmount = settings?.MaxTransferAmount ?? 50000m;
        }
    }
}
