using Microsoft.EntityFrameworkCore;
using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
        public DbSet<CompanyWallet> CompanyWallets { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<SupportMessage> SupportMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting
            {
                Id = 1,
                FeePercentage = 1.5m,
                MinTransferAmount = 10,
                MaxTransferAmount = 50000,
                IsActive = true
            }
        );

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.ReferredByUser)
                .WithMany()
                .HasForeignKey(u => u.ReferredByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(t => t.User)
                .WithMany(u => u.TransferRequests)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(t => t.VerifiedByAdmin)
                .WithMany()
                .HasForeignKey(t => t.VerifiedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(t => t.CompletedByAdmin)
                .WithMany()
                .HasForeignKey(t => t.CompletedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(t => t.RejectedByAdmin)
                .WithMany()
                .HasForeignKey(t => t.RejectedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(t => t.CompanyWallet)
                .WithMany(w => w.TransferRequests)
                .HasForeignKey(t => t.CompanyWalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.TransferRequest)
                .WithOne(r => r.Transaction)
                .HasForeignKey<Transaction>(t => t.TransferRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<CompanyWallet>()
                .HasIndex(w => w.WalletNumber)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.ReferenceNumber)
                .IsUnique();

            modelBuilder.Entity<SupportMessage>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupportMessage>()
                .HasIndex(m => new { m.UserId, m.CreatedAt });

            modelBuilder.Entity<SupportMessage>()
                .HasIndex(m => new { m.IsRead, m.IsFromAdmin });
        }

    }
}
