using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wevo_Pay_Project.Models
{
    public class SystemSetting
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal FeePercentage { get; set; } = 1.5m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinTransferAmount { get; set; } = 10;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxTransferAmount { get; set; } = 50000;

        public bool IsActive { get; set; } = true;
    }
}