using System.ComponentModel.DataAnnotations;

namespace Wevo_Pay_Project.Models
{
    public class CompanyWallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string WalletName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string WalletNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public ICollection<TransferRequest> TransferRequests { get; set; }
            = new List<TransferRequest>();
    }
}
