using Wevo_Pay_Project.Models;

namespace Wevo_Pay_Project.DTOs
{
    public class UserDashboardDto
    {
        public int PendingTransfers { get; set; }

        public int CompletedTransfers { get; set; }

        public int TotalTransfers { get; set; }

        public decimal TotalAmount { get; set; }
        public List<TransferRequest> RecentTransfers { get; set; } = new();
    }
}