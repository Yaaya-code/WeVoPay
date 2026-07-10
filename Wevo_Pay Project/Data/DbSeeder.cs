using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Data
{
    public static class DbSeeder
    {
        public const string AdminUserName = "admin";
        public const string AdminEmail = "admin@wevopay.com";
        public const string AdminPassword = "Admin@123";
        public const string AdminPhone = "01099999999";

        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            await context.Database.MigrateAsync();

            var admin = await context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserName == AdminUserName ||
                    u.Email == AdminEmail);

            if (admin == null)
            {
                admin = new User
                {
                    FullName = "System Admin",
                    UserName = AdminUserName,
                    Email = AdminEmail,
                    PhoneNumber = AdminPhone,
                    Role = UserRole.Admin,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                admin.PasswordHash = hasher.HashPassword(admin, AdminPassword);
                context.Users.Add(admin);
            }
            else
            {
                admin.FullName = "System Admin";
                admin.UserName = AdminUserName;
                admin.Email = AdminEmail;
                admin.PhoneNumber = AdminPhone;
                admin.Role = UserRole.Admin;
                admin.IsVerified = true;
                admin.PasswordHash = hasher.HashPassword(admin, AdminPassword);
            }

            await context.SaveChangesAsync();
        }
    }
}
