using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface ICompanyWalletService
    {
        Task<List<CompanyWallet>> GetAllAsync();

        Task<CompanyWallet?> GetByIdAsync(int id);

        Task<CompanyWallet> CreateAsync(CompanyWallet wallet);

        Task<bool> UpdateAsync(CompanyWallet wallet);

        Task<bool> ToggleStatusAsync(int id);
    }
}