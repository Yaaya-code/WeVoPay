using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class CompanyWalletService : ICompanyWalletService
    {
        private readonly AppDbContext _context;

        public CompanyWalletService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<CompanyWallet>> GetAllAsync()
        {
            return await _context.CompanyWallets
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }


        public async Task<CompanyWallet?> GetByIdAsync(int id)
        {
            return await _context.CompanyWallets
                .FirstOrDefaultAsync(w => w.Id == id);
        }


        public async Task<CompanyWallet> CreateAsync(CompanyWallet wallet)
        {
            wallet.IsActive = true;
            wallet.CreatedAt = DateTime.UtcNow;
            wallet.WalletName = wallet.WalletName.Trim();
            wallet.WalletNumber = wallet.WalletNumber.Trim();

            var exists = await _context.CompanyWallets
                .AnyAsync(w => w.WalletNumber == wallet.WalletNumber);

            if (exists)
                throw new Exception("Wallet number already exists.");

            _context.CompanyWallets.Add(wallet);

            await _context.SaveChangesAsync();

            return wallet;
        }


        public async Task<bool> UpdateAsync(CompanyWallet wallet)
        {
            var existingWallet = await _context.CompanyWallets
                .FirstOrDefaultAsync(w => w.Id == wallet.Id);

            if (existingWallet == null)
                return false;

            wallet.WalletName = wallet.WalletName.Trim();
            wallet.WalletNumber = wallet.WalletNumber.Trim();

            var exists = await _context.CompanyWallets
                .AnyAsync(w => w.WalletNumber == wallet.WalletNumber
                            && w.Id != wallet.Id);

            if (exists)
                return false;


            existingWallet.WalletName = wallet.WalletName;
            existingWallet.WalletNumber = wallet.WalletNumber;


            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> ToggleStatusAsync(int id)
        {
            var wallet = await _context.CompanyWallets
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wallet == null)
                return false;


            wallet.IsActive = !wallet.IsActive;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}