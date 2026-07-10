using System.ComponentModel.DataAnnotations;

namespace Wevo_Pay_Project.DTOs
{
    public class UpdateCompanyWalletDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string WalletName { get; set; }

        [Required]
        [MaxLength(20)]
        public string WalletNumber { get; set; }
    }
}
