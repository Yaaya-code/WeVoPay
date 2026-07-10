using System.ComponentModel.DataAnnotations;

namespace Wevo_Pay_Project.DTOs
{
    public class LoginDto
    {
        [Required]
        public string EmailOrUserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
