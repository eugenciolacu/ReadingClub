using System.ComponentModel.DataAnnotations;

namespace ReadingClub.Infrastructure.DTO
{
    public class UserEmailDto
    {
        [Required]
        public string UserEmail { get; set; } = null!;
    }
}
