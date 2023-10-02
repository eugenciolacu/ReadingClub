namespace ReadingClub.Infrastructure.DTO.Book
{
    public class BookToReadingListDto
    {
        public BookToReadingListDto(string userEmail, int bookId, bool isRead = false) {
            UserEmail = userEmail;
            BookId = bookId;
            IsRead = isRead;
        }

        public string UserEmail { get; set; }
        public int BookId { get; set; }
        public bool IsRead { get; set; }
    }
}
