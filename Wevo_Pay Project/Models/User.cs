using System.ComponentModel.DataAnnotations;
using Wevo_Pay_Project.Enums;

namespace Wevo_Pay_Project.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public ICollection<TransferRequest> TransferRequests { get; set; }
            = new List<TransferRequest>();

        public UserRole Role { get; set; } = UserRole.User;
    }
}