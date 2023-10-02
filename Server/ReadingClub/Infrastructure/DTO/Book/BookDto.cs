using System.ComponentModel.DataAnnotations;

namespace ReadingClub.Infrastructure.DTO.Book
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Authors { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Cover { get; set; } = null!;
        public string CoverName { get; set; } = null!;
        public string CoverMime { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string AddedByUserName { get; set; } = null!;
        public bool IsInReadingList { get; set; } = false;
        public bool IsRead { get; set; } = false;
    }
}
