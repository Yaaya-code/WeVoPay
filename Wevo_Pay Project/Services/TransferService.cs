using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class TransferService : ITransferService
    {
        public static readonly TimeSpan PendingTimeout = TimeSpan.FromHours(1);

        private readonly AppDbContext _context;

        public TransferService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransferRequest>> GetAllTransfersAsync()
        {
            await CancelExpiredPendingTransfersAsync();

            return await _context.TransferRequests
                .Include(t => t.User)
                .Include(t => t.CompanyWallet)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TransferRequest>> GetUserTransfersAsync(
            int userId,
            string? search,
            TransferStatus? status,
            int page,
            int pageSize)
        {
            await CancelExpiredPendingTransfersAsync();

            var query = _context.TransferRequests
                .Include(t => t.CompanyWallet)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.CompanyWallet.WalletName.Contains(search) ||
                    t.InstaPayAddress.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserTransfersCountAsync(
            int userId,
            string? search,
            TransferStatus? status)
        {
            var query = _context.TransferRequests
                .Include(t => t.CompanyWallet)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.CompanyWallet.WalletName.Contains(search) ||
                    t.InstaPayAddress.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<TransferRequest?> GetUserTransferByIdAsync(int transferId, int userId)
        {
            await CancelExpiredPendingTransfersAsync();

            return await _context.TransferRequests
                .Include(t => t.User)
                .Include(t => t.CompanyWallet)
                .Include(t => t.Transaction)
                .FirstOrDefaultAsync(t => t.Id == transferId && t.UserId == userId);
        }

        public async Task<TransferRequest> CreateTransferAsync(int userId, CreateTransferDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var wallet = await _context.CompanyWallets
                .FirstOrDefaultAsync(w => w.Id == dto.CompanyWalletId && w.IsActive);
            if (wallet == null)
                throw new Exception("Company wallet not found.");

            var settings = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.IsActive);
            if (settings == null)
                throw new Exception("System settings not found.");

            if (string.IsNullOrWhiteSpace(dto.InstaPayAddress))
                throw new Exception("InstaPay address is required.");

            dto.InstaPayAddress = dto.InstaPayAddress.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Notes))
                dto.Notes = dto.Notes.Trim();

            if (dto.TransferAmount < settings.MinTransferAmount ||
                dto.TransferAmount > settings.MaxTransferAmount)
            {
                throw new Exception(
                    $"Transfer amount must be between {settings.MinTransferAmount} and {settings.MaxTransferAmount}.");
            }

            decimal fee = Math.Round(dto.TransferAmount * settings.FeePercentage / 100, 2);
            decimal totalAmount = dto.TransferAmount + fee;

            var transfer = new TransferRequest
            {
                UserId = userId,
                CompanyWalletId = wallet.Id,
                InstaPayAddress = dto.InstaPayAddress,
                TransferAmount = dto.TransferAmount,
                Fee = fee,
                TotalAmount = totalAmount,
                FeePercentage = settings.FeePercentage,
                Notes = dto.Notes,
                Status = TransferStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.TransferRequests.Add(transfer);
            await _context.SaveChangesAsync();
            return transfer;
        }

        public async Task<bool> VerifyTransferAsync(int id, int adminId)
        {
            await CancelExpiredPendingTransfersAsync();

            var transfer = await _context.TransferRequests
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
                return false;

            if (transfer.Status != TransferStatus.Pending)
                return false;

            transfer.Status = TransferStatus.Verified;
            transfer.VerifiedAt = DateTime.UtcNow;
            transfer.VerifiedByAdminId = adminId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteTransferAsync(int id, int adminId)
        {
            var transfer = await _context.TransferRequests
                .Include(t => t.Transaction)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
                return false;

            if (transfer.Status != TransferStatus.Verified)
                return false;

            if (transfer.Transaction != null)
                return false;

            var transaction = new Transaction
            {
                TransferRequestId = transfer.Id,
                Amount = transfer.TransferAmount,
                Status = TransactionStatus.Completed,
                ReferenceNumber = $"WP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            transfer.Status = TransferStatus.Completed;
            transfer.CompletedAt = DateTime.UtcNow;
            transfer.CompletedByAdminId = adminId;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTransferAsync(int id, int adminId)
        {
            var transfer = await _context.TransferRequests
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
                return false;

            if (transfer.Status != TransferStatus.Pending &&
                transfer.Status != TransferStatus.Verified)
            {
                return false;
            }

            transfer.Status = TransferStatus.Rejected;
            transfer.RejectedByAdminId = adminId;
            transfer.RejectedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<TransferRequest> Transfers, int TotalCount)> GetPendingTransfersAsync(
            string? search,
            int page,
            int pageSize)
        {
            await CancelExpiredPendingTransfersAsync();
            return await GetTransfersByStatusAsync(TransferStatus.Pending, search, page, pageSize);
        }

        public async Task<(List<TransferRequest> Transfers, int TotalCount)> GetVerifiedTransfersAsync(
            string? search,
            int page,
            int pageSize)
        {
            return await GetTransfersByStatusAsync(TransferStatus.Verified, search, page, pageSize);
        }

        public async Task<(List<TransferRequest> Transfers, int TotalCount)> GetCompletedTransfersAsync(
            string? search,
            int page,
            int pageSize)
        {
            return await GetTransfersByStatusAsync(TransferStatus.Completed, search, page, pageSize);
        }

        public async Task<(List<TransferRequest> Transfers, int TotalCount)> GetRejectedTransfersAsync(
            string? search,
            int page,
            int pageSize)
        {
            return await GetTransfersByStatusAsync(TransferStatus.Rejected, search, page, pageSize);
        }

        public async Task<TransferRequest?> GetTransferByIdAsync(int id)
        {
            await CancelExpiredPendingTransfersAsync();

            return await _context.TransferRequests
                .Include(t => t.User)
                .Include(t => t.CompanyWallet)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> CancelExpiredPendingTransfersAsync()
        {
            var cutoff = DateTime.UtcNow - PendingTimeout;

            var expired = await _context.TransferRequests
                .Where(t => t.Status == TransferStatus.Pending && t.CreatedAt <= cutoff)
                .ToListAsync();

            if (expired.Count == 0)
                return 0;

            foreach (var transfer in expired)
            {
                transfer.Status = TransferStatus.Cancelled;
            }

            await _context.SaveChangesAsync();
            return expired.Count;
        }

        private async Task<(List<TransferRequest> Transfers, int TotalCount)> GetTransfersByStatusAsync(
            TransferStatus status,
            string? search,
            int page,
            int pageSize)
        {
            var query = _context.TransferRequests
                .Include(t => t.User)
                .Include(t => t.CompanyWallet)
                .Where(t => t.Status == status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.User.FullName.Contains(search) ||
                    t.CompanyWallet.WalletName.Contains(search) ||
                    t.InstaPayAddress.Contains(search));
            }

            var totalCount = await query.CountAsync();

            List<TransferRequest> transfers;
            if (status == TransferStatus.Completed)
            {
                transfers = await query
                    .OrderByDescending(t => t.CompletedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else if (status == TransferStatus.Rejected)
            {
                transfers = await query
                    .OrderByDescending(t => t.RejectedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                transfers = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return (transfers, totalCount);
        }
    }
}
