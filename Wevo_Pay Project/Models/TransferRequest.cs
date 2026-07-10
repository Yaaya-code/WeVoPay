using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wevo_Pay_Project.Enums;

namespace Wevo_Pay_Project.Models
{
    public class TransferRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("CompanyWallet")]
        public int CompanyWalletId { get; set; }

        [Required]
        [MaxLength(100)]
        public string InstaPayAddress { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TransferAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal FeePercentage { get; set; }

        public TransferStatus Status { get; set; } = TransferStatus.Pending;

        [MaxLength(250)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public User User { get; set; } = null!;

        public CompanyWallet CompanyWallet { get; set; } = null!;

        public Transaction? Transaction { get; set; }

        public int? VerifiedByAdminId { get; set; }

        public User? VerifiedByAdmin { get; set; }

        public int? CompletedByAdminId { get; set; }

        public User? CompletedByAdmin { get; set; }

        public int? RejectedByAdminId { get; set; }

        public User? RejectedByAdmin { get; set; }

        public DateTime? RejectedAt { get; set; }
    }
}
