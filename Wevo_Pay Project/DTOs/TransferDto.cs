namespace Wevo_Pay_Project.DTOs
{
    public class TransferDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string InstaPayAddress { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}