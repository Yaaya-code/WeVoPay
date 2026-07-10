using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return new AdminDashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),

                TotalWallets = await _context.CompanyWallets.CountAsync(),

                PendingTransfers = await _context.TransferRequests
                    .CountAsync(t => t.Status == TransferStatus.Pending),

                VerifiedTransfers = await _context.TransferRequests
                    .CountAsync(t => t.Status == TransferStatus.Verified),

                CompletedTransfers = await _context.TransferRequests
                    .CountAsync(t => t.Status == TransferStatus.Completed),

                RejectedTransfers = await _context.TransferRequests
                    .CountAsync(t => t.Status == TransferStatus.Rejected),

                TotalRevenue = await _context.TransferRequests
                    .Where(t => t.Status == TransferStatus.Completed)
                    .SumAsync(t => (decimal?)t.Fee) ?? 0,

                TodayRevenue = await _context.TransferRequests
                    .Where(t => t.Status == TransferStatus.Completed &&
                                t.CompletedAt.HasValue &&
                                t.CompletedAt >= today &&
                                t.CompletedAt < tomorrow)
                    .SumAsync(t => (decimal?)t.Fee) ?? 0
            };
        }
    }
}
