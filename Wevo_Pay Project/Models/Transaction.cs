using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wevo_Pay_Project.Enums;

namespace Wevo_Pay_Project.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [ForeignKey("TransferRequest")]
        public int TransferRequestId { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }


        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;


        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }


        public DateTime CreatedAt { get; set; }


        public DateTime? CompletedAt { get; set; }


        public TransferRequest TransferRequest { get; set; }
    }
}