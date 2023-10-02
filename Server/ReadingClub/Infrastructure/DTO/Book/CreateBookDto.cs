using System.ComponentModel.DataAnnotations;

namespace ReadingClub.Infrastructure.DTO.Book
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Authors { get; set; } = null!;
        public string? ISBN { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Cover { get; set; } = null!;
        public string? CoverName { get; set; } = null!;
        [Required]
        public string File { get; set; } = null!;
        [Required]
        public string FileName { get; set; } = null!;
        [Required]
        public string AddedByEmail { get; set; } = null!;
    }
}
