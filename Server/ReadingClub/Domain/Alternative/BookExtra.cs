namespace ReadingClub.Domain.Alternative
{
    public class BookExtra : Book
    {
        public string AddedByUserName { get; set; } = null!;
        public bool IsInReadingList { get; set; } = false;
        public bool IsRead { get; set; } = false;
    }
}
