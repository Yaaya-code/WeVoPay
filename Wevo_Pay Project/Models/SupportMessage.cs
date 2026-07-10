using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wevo_Pay_Project.Models
{
    public class SupportMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Body { get; set; } = string.Empty;

        public bool IsFromAdmin { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; } = null!;
    }
}
