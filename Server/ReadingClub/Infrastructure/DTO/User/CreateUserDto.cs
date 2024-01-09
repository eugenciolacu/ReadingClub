using System.ComponentModel.DataAnnotations;

namespace ReadingClub.Infrastructure.DTO.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "The User name field is required.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "The Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email address.")]
        public string Email
        {
            get { return _email; }
            set { _email = value.ToLower(); }
        }

        [Required]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "The Confirm password field is required.")]
        [Compare("Password", ErrorMessage = "The Password and Confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        private string _email = string.Empty;
    }
}
