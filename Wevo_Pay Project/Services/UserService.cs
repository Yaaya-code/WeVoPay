using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Models;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phone);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            if (await EmailExistsAsync(dto.Email))
                throw new Exception("Email already exists.");

            if (await PhoneExistsAsync(dto.PhoneNumber))
                throw new Exception("Phone number already exists.");

            if (await UserNameExistsAsync(dto.UserName))
                throw new Exception("Username already exists.");

            dto.Email = dto.Email.Trim();
            dto.UserName = dto.UserName.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();


            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };


            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);


            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == dto.EmailOrUserName ||
                u.UserName == dto.EmailOrUserName);

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }

        public async Task<UserDashboardDto> GetDashboardAsync(int userId)
        {
            var transfers = _context.TransferRequests
                .Where(t => t.UserId == userId);

            return new UserDashboardDto
            {
                PendingTransfers = await transfers
                    .CountAsync(t => t.Status == TransferStatus.Pending),

                CompletedTransfers = await transfers
                    .CountAsync(t => t.Status == TransferStatus.Completed),

                TotalTransfers = await transfers.CountAsync(),

                TotalAmount = await transfers
                    .SumAsync(t => (decimal?)t.TransferAmount) ?? 0 ,

                RecentTransfers = await transfers
                    .Include(t => t.CompanyWallet)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };
        }

    }
}