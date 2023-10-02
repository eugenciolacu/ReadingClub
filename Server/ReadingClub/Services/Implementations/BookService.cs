using AutoMapper;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository, IUserRepository userRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> Create(CreateBookDto createBookDto)
        {
            var addedBy = await _userRepository.GetUserIdByEmail(createBookDto.AddedByEmail);

            var book = _mapper.Map<Book>(createBookDto);
            book.AddedBy = addedBy;

            var createdBook = await _bookRepository.Create(book);

            var createdBookExtra = await _bookRepository.GetExtra(createdBook.Id);

            var bookDto = _mapper.Map<BookDto>(createdBookExtra);

            return bookDto;
        }

        public async Task<BookDto> Delete(int id)
        {
            var anonymousUserId = await _userRepository.GetUserIdByEmail("anonymous");

            var book = await _bookRepository.Get(id);

            book.AddedBy = anonymousUserId;

            var updatedBook = await _bookRepository.DeleteUpdate(book);

            var updatedBookExtra = await _bookRepository.GetExtra(updatedBook.Id);

            var bookDto = _mapper.Map<BookDto>(updatedBookExtra);

            return bookDto;
        }

        public async Task<BookDto> Get(int id)
        {
            var book = await _bookRepository.GetExtra(id);

            var bookDto = _mapper.Map<BookDto>(book);

            return bookDto;
        }

        public async Task<PagedResponse<BookDto>> GetPagedAdminPage(PagedRequest pagedRequest)
        {
            var rawPagedResponse = await _bookRepository.GetPagedAdminPage(pagedRequest);

            var pagedResponse = new PagedResponse<BookDto>(_mapper.Map<List<BookDto>>(rawPagedResponse.Items), rawPagedResponse.TotalItems);

            return pagedResponse;
        }

        public async Task<PagedResponse<BookDto>> GetPagedSearchPage(PagedRequest pagedRequest)
        {
            var rawPagedResponse = await _bookRepository.GetPagedSearchPage(pagedRequest);

            var pagedResponse = new PagedResponse<BookDto>(_mapper.Map<List<BookDto>>(rawPagedResponse.Items), rawPagedResponse.TotalItems);

            return pagedResponse;
        }

        public async Task<PagedResponse<BookDto>> GetPagedReadingListPage(PagedRequest pagedRequest)
        {
            var rawPagedResponse = await _bookRepository.GetPagedReadingListPage(pagedRequest);

            var pagedResponse = new PagedResponse<BookDto>(_mapper.Map<List<BookDto>>(rawPagedResponse.Items), rawPagedResponse.TotalItems);

            return pagedResponse;
        }

        public async Task<BookDto> Update(UpdateBookDto updateBookDto)
        {
            var book = _mapper.Map<Book>(updateBookDto);

            var updatedBook = await _bookRepository.Update(book, updateBookDto.IsCoverEdited, updateBookDto.IsFileEdited);

            var updatedBookExtra = await _bookRepository.GetExtra(updatedBook.Id);

            var bookDto = _mapper.Map<BookDto>(updatedBookExtra);

            return bookDto;
        }

        public async Task AddToReadingList(BookToReadingListDto bookToReadingListDto)
        {
            await _bookRepository.AddToReadingList(bookToReadingListDto);
        }

        public async Task RemoveFromReadingList(BookToReadingListDto bookToReadingListDto)
        {
            await _bookRepository.RemoveFromReadingList(bookToReadingListDto);
        }

        public async Task MarkAsReadOrUnread(BookToReadingListDto bookToReadingListDto)
        {
            await _bookRepository.MarkAsReadOrUnread(bookToReadingListDto);
        }

        public async Task<BookStatisticsDto> GetStatistics(string userEmail)
        {
            return await _bookRepository.GetStatistics(userEmail);
        }

        public async Task<string?> GetBookForDownload(int id)
        {
            var bookAsByte = await _bookRepository.GetBookForDownload(id);
        
            if (bookAsByte == null)
            {
                return null;
            }

            return Convert.ToBase64String(bookAsByte);
        }
    }
}
