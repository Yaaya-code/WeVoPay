namespace Wevo_Pay_Project.DTOs
{
    public class UpdateSettingDto
    {
        public decimal FeePercentage { get; set; }

        public decimal MinTransferAmount { get; set; }

        public decimal MaxTransferAmount { get; set; }
    }
}
