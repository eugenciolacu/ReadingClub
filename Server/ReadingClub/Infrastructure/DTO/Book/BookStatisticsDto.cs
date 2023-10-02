namespace ReadingClub.Infrastructure.DTO.Book
{
    public class BookStatisticsDto
    {
        public int TotalBooks { get; set; }
        public int UploadedByUser { get; set; }
        public int InUserReadingList { get; set; }
        public int ReadByUser { get; set; }
    }
}
