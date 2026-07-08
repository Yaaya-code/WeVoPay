using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompanyWalletController : Controller
    {
        private readonly ICompanyWalletService _walletService;

        public CompanyWalletController(ICompanyWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var wallets = await _walletService.GetAllAsync();

            return View(wallets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyWallet wallet)
        {
            if (!ModelState.IsValid)
                return View(wallet);

            try
            {
                await _walletService.CreateAsync(wallet);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(wallet);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var wallet = await _walletService.GetByIdAsync(id);

            if (wallet == null)
                return NotFound();

            return View(wallet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyWallet wallet)
        {
            if (!ModelState.IsValid)
                return View(wallet);

            var result = await _walletService.UpdateAsync(wallet);

            if (!result)
            {
                ModelState.AddModelError("", "Wallet number already exists or wallet not found.");
                return View(wallet);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _walletService.ToggleStatusAsync(id);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}