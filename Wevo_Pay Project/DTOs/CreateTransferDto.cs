using System.ComponentModel.DataAnnotations;

namespace Wevo_Pay_Project.DTOs
{
    public class CreateTransferDto
    {
        [Required]
        public int CompanyWalletId { get; set; }

        [Required]
        [MaxLength(100)]
        public string InstaPayAddress { get; set; }

        [Required]
        [Range(1, 100000)]
        public decimal TransferAmount { get; set; }

        [MaxLength(250)]
        public string? Notes { get; set; }
    }
}
