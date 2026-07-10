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
            dto.FullName = dto.FullName.Trim();

            int? referredByUserId = null;

            if (dto.WasReferred)
            {
                if (string.IsNullOrWhiteSpace(dto.ReferralCode))
                    throw new Exception("Please enter the referral link or username.");

                var referrerUserName = ExtractReferralUserName(dto.ReferralCode);

                if (string.IsNullOrWhiteSpace(referrerUserName))
                    throw new Exception("Invalid referral link or username.");

                if (string.Equals(referrerUserName, dto.UserName, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("You cannot refer yourself.");

                var referrer = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.UserName.ToLower() == referrerUserName.ToLower() &&
                        u.Role == UserRole.User);

                if (referrer == null)
                    throw new Exception("Referral not found. Check the link or username and try again.");

                referredByUserId = referrer.Id;
            }

            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow,
                ReferredByUserId = referredByUserId
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        private static string ExtractReferralUserName(string input)
        {
            input = input.Trim();

            var refIndex = input.IndexOf("/ref/", StringComparison.OrdinalIgnoreCase);
            if (refIndex >= 0)
            {
                var after = input[(refIndex + 5)..];
                after = after.Split('?', '#')[0].Trim('/');
                return after;
            }

            if (input.Contains("ref=", StringComparison.OrdinalIgnoreCase))
            {
                var query = input.Contains('?')
                    ? input[(input.IndexOf('?') + 1)..]
                    : input;

                foreach (var part in query.Split('&'))
                {
                    var kv = part.Split('=', 2);
                    if (kv.Length == 2 &&
                        kv[0].Equals("ref", StringComparison.OrdinalIgnoreCase))
                    {
                        return Uri.UnescapeDataString(kv[1]).Trim();
                    }
                }
            }

            return input.Trim().TrimStart('@');
        }

        public async Task<User?> LoginAsync(LoginDto dto)
        {
            var login = (dto.EmailOrUserName ?? string.Empty).Trim();
            var password = dto.Password ?? string.Empty;

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == login.ToLower() ||
                u.UserName.ToLower() == login.ToLower());

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            if (result == PasswordVerificationResult.Failed)
                return null;

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

        public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found.");

            dto.FullName = dto.FullName.Trim();
            dto.Email = dto.Email.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();

            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.Id != userId);

            if (emailTaken)
                throw new Exception("This email is already used by another account.");

            var phoneTaken = await _context.Users
                .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber && u.Id != userId);

            if (phoneTaken)
                throw new Exception("This phone number is already used by another account.");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found.");

            var verify = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.CurrentPassword);

            if (verify == PasswordVerificationResult.Failed)
                throw new Exception("Current password is incorrect.");

            if (dto.NewPassword.Length < 6)
                throw new Exception("New password must be at least 6 characters.");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new Exception("New password and confirmation do not match.");

            var sameAsOld = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.NewPassword);

            if (sameAsOld != PasswordVerificationResult.Failed)
                throw new Exception("New password must be different from your current password.");

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            await _context.SaveChangesAsync();
        }
    }
}
