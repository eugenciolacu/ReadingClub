using System.ComponentModel.DataAnnotations;

namespace ReadingClub.Infrastructure.DTO.User
{
    public class UserLoginDto
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
