namespace Wevo_Pay_Project.DTOs
{
    public class UserSettingsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }

        public UpdateProfileDto Profile { get; set; } = new();
        public ChangePasswordDto Password { get; set; } = new();
    }
}
