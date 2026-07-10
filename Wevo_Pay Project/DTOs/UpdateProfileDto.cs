using System.ComponentModel.DataAnnotations;

namespace Wevo_Pay_Project.DTOs
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [MaxLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
