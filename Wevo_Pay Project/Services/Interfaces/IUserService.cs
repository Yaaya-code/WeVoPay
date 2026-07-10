using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int id);

        Task<User> RegisterAsync(RegisterDto dto);

        Task<User?> LoginAsync(LoginDto dto);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> PhoneExistsAsync(string phone);

        Task<bool> UserNameExistsAsync(string userName);

        Task<UserDashboardDto> GetDashboardAsync(int userId);

        Task UpdateProfileAsync(int userId, UpdateProfileDto dto);

        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
