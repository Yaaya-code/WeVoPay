using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.TransferRequest)
                    .ThenInclude(r => r.User)
                .Include(t => t.TransferRequest)
                    .ThenInclude(r => r.CompanyWallet)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.TransferRequest)
                    .ThenInclude(r => r.User)
                .Include(t => t.TransferRequest)
                    .ThenInclude(r => r.CompanyWallet)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
