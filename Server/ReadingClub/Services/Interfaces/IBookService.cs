using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.DTO.User;

namespace ReadingClub.Services.Interfaces
{
    public interface IBookService
    {
        Task<BookDto> Create(CreateBookDto createBookDto);
        Task<BookDto> Delete(int id);
        Task<BookDto> Get(int id);
        Task<PagedResponse<BookDto>> GetPagedAdminPage(PagedRequest pagedRequest);
        Task<PagedResponse<BookDto>> GetPagedSearchPage(PagedRequest pagedRequest);
        Task<PagedResponse<BookDto>> GetPagedReadingListPage(PagedRequest pagedRequest);
        Task<BookDto> Update(UpdateBookDto updateBookDto);
        Task AddToReadingList(BookToReadingListDto bookToReadingListDto);
        Task RemoveFromReadingList(BookToReadingListDto bookToReadingListDto);
        Task MarkAsReadOrUnread(BookToReadingListDto bookToReadingListDto);
        Task<BookStatisticsDto> GetStatistics(string userEmail);
        Task<string?> GetBookForDownload(int id);
    }
}
