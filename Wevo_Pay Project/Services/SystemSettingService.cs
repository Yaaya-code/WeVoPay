using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly AppDbContext _context;

        public SystemSettingService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<SystemSetting?> GetAsync()
        {
            return await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.IsActive);
        }


        public async Task<bool> UpdateAsync(SystemSetting setting)
        {
            if (setting.FeePercentage < 0)
                return false;

            if (setting.MinTransferAmount <= 0)
                return false;

            if (setting.MaxTransferAmount <= setting.MinTransferAmount)
                return false;


            var existingSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Id == setting.Id);

            if (existingSetting == null)
                return false;


            existingSetting.FeePercentage = setting.FeePercentage;
            existingSetting.MinTransferAmount = setting.MinTransferAmount;
            existingSetting.MaxTransferAmount = setting.MaxTransferAmount;


            await _context.SaveChangesAsync();

            return true;
        }
    }
}