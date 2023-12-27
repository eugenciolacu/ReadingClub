using AutoMapper;
using Moq;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Services.Implementations;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.UnitTests.Services.Implementations
{
    public class BookServiceTest
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IBookService _bookService;
        public BookServiceTest() 
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();

            _bookService = new BookService(
                _mockBookRepository.Object,
                _mockUserRepository.Object,
                _mockMapper.Object
            );
        }

        #region Create

        #endregion

        #region Delete

        #endregion

        #region Get

        #endregion

        #region GetPagedAdminPage

        #endregion

        #region GetPagedSearchPage

        #endregion

        #region GetPagedReadingListPage

        #endregion

        #region Update

        #endregion

        #region AddToReadingList

        #endregion

        #region RemoveFromReadingList

        #endregion

        #region MarkAsReadOrUnread

        #endregion

        #region GetStatistics

        #endregion

        #region GetBookForDowload

        #endregion
    }
}
