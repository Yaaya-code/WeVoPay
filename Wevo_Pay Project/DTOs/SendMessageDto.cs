using System.ComponentModel.DataAnnotations;

namespace Wevo_Pay_Project.DTOs
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "Message cannot be empty.")]
        [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        [Display(Name = "Message")]
        public string Body { get; set; } = string.Empty;

        public int? TargetUserId { get; set; }
    }
}
