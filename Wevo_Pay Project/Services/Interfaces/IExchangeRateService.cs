using Wevo_Pay_Project.DTOs;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ExchangeRatesViewModel> GetRatesAsync();
    }
}
