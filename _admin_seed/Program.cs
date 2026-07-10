using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wevo_Pay_Project.Data;
using Wevo_Pay_Project.Enums;
using Wevo_Pay_Project.Models;

var basePath = args[0];
var config = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile("appsettings.json").Build();
var conn = config.GetConnectionString("conString") ?? config.GetConnectionString("ConString");
var services = new ServiceCollection();
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));
services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
using var sp = services.BuildServiceProvider();
using var scope = sp.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
Console.WriteLine("Connect=" + db.Database.CanConnect());
var admin = await db.Users.FirstOrDefaultAsync(u => u.UserName == "admin" || u.Email == "admin@wevopay.com");
if (admin == null) {
  admin = new User { FullName="System Admin", UserName="admin", Email="admin@wevopay.com", PhoneNumber="01099999999", Role=UserRole.Admin, IsVerified=true, CreatedAt=DateTime.UtcNow };
  admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
  db.Users.Add(admin);
  Console.WriteLine("CREATE");
} else {
  admin.UserName="admin"; admin.Email="admin@wevopay.com"; admin.PhoneNumber="01099999999"; admin.Role=UserRole.Admin; admin.IsVerified=true;
  admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
  Console.WriteLine("UPDATE id="+admin.Id);
}
await db.SaveChangesAsync();
var check = await db.Users.FirstAsync(u => u.UserName=="admin");
Console.WriteLine("Role="+check.Role+" Verify="+hasher.VerifyHashedPassword(check, check.PasswordHash, "Admin@123"));
