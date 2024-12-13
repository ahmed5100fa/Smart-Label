using System.ComponentModel.DataAnnotations;

namespace SmartLabel.models
{
    public class registerUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
