using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;
        private readonly ICompanyWalletService _walletService;

        public TransferController(
            ITransferService transferService,
            ICompanyWalletService walletService)
        {
            _transferService = transferService;
            _walletService = walletService;
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var wallets = await _walletService.GetAllAsync();

            ViewBag.Wallets = wallets
                .Where(w => w.IsActive)
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadWalletsAsync();

                return View(dto);
            }


            try
            {
                int userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!
                );

                await _transferService.CreateTransferAsync(userId, dto);

                return RedirectToAction(nameof(MyTransfers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                await LoadWalletsAsync();

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyTransfers()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );


            var transfers = await _transferService
                .GetUserTransfersAsync(userId);


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

        private async Task LoadWalletsAsync()
        {
            var wallets = await _walletService.GetAllAsync();

            ViewBag.Wallets = wallets
                .Where(w => w.IsActive)
                .ToList();
        }


    }
}