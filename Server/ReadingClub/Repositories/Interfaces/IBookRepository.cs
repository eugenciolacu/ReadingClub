using ReadingClub.Domain;
using ReadingClub.Domain.Alternative;
using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO.Book;

namespace ReadingClub.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> Create(Book book);
        Task<Book> Get(int id);
        Task<byte[]?> GetBookForDownload(int id);
        Task<BookExtra> GetExtra(int id);
        Task<PagedResponse<BookExtra>> GetPagedAdminPage(PagedRequest pagedRequest);
        Task<PagedResponse<BookExtra>> GetPagedSearchPage(PagedRequest pagedRequest);
        Task<PagedResponse<BookExtra>> GetPagedReadingListPage(PagedRequest pagedRequest);
        Task<Book> Update(Book book, bool isCoverEdited, bool isFileEdited);
        Task<Book> DeleteUpdate(Book book);
        Task ClearBooksForUserBeforeDelete(string userEmail, int anonymousId);
        Task AddToReadingList(BookToReadingListDto bookToReadingListDto);
        Task RemoveFromReadingList(BookToReadingListDto bookToReadingListDto);
        Task MarkAsReadOrUnread(BookToReadingListDto bookToReadingListDto);
        Task<BookStatisticsDto> GetStatistics(string userEmail);
    }
}