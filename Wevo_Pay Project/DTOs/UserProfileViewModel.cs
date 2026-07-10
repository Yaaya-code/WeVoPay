namespace Wevo_Pay_Project.DTOs
{
    public class UserProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }

        public int TotalTransfers { get; set; }
        public int CompletedTransfers { get; set; }
        public int PendingTransfers { get; set; }
    }
}
