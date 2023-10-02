using System.ComponentModel.DataAnnotations.Schema;

namespace ReadingClub.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Authors { get; set; } = null!;
        public string? ISBN { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public byte[]? Cover { get => _cover; set => _cover = value != null && value.Length > 0 ? value : null; }
        public string? CoverName { get; set; } = null!;
        public string? CoverMime { get; set; } = null!;
        public byte[] File { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int AddedBy { get; set; }

        private byte[]? _cover;
    }
}
