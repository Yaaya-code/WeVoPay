using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Controllers
{
    [Authorize]
    public class RatesController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;

        public RatesController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _exchangeRateService.GetRatesAsync();
            return View(model);
        }
    }
}
