using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface ISystemSettingService
    {
        Task<SystemSetting?> GetAsync();

        Task<bool> UpdateAsync(SystemSetting setting);
    }
}