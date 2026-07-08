using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var transactions = await _transactionService.GetAllAsync();

            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);

            if (transaction == null)
                return NotFound();

            return View(transaction);
        }
    }
}