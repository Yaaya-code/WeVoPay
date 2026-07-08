namespace Wevo_Pay_Project.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }

        public int TotalWallets { get; set; }

        public int PendingTransfers { get; set; }

        public int VerifiedTransfers { get; set; }

        public int CompletedTransfers { get; set; }

        public int RejectedTransfers { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TodayRevenue { get; set; }
    }
}