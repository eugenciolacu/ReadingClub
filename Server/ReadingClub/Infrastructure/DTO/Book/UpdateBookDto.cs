using System.ComponentModel.DataAnnotations;

namespace ReadingClub.Infrastructure.DTO.Book
{
    public class UpdateBookDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Authors { get; set; } = null!;
        public string? ISBN { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Cover { get; set; } = null!;
        public string? CoverName { get; set; } = null!;
        public bool IsCoverEdited { get; set; } = false;
        [Required]
        public string File { get; set; } = null!;
        [Required]
        public string FileName { get; set; } = null!;
        public bool IsFileEdited { get; set; } = false;
    }
}
