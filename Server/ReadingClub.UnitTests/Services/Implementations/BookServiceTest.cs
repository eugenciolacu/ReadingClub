using AutoMapper;
using Moq;
using ReadingClub.Domain;
using ReadingClub.Domain.Alternative;
using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.Profile;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Services.Implementations;
using ReadingClub.Services.Interfaces;
using System.Text;

namespace ReadingClub.UnitTests.Services.Implementations
{
    public class BookServiceTest
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;

        private readonly IMapper _mapper;
        
        private readonly IBookService _bookService;

        // small file converted in base64 string with meme (see in ReadingClub.FilesForTesting project)
        private readonly string _coverAsBase64WithMeme = FilesForTesting.Files.CoverAsBase64WithMeme;
        private readonly string _coverAsBase64WithoutMeme = FilesForTesting.Files.CoverAsBase64WithoutMeme;
        private readonly string _coverAsBase64Meme = FilesForTesting.Files.CoverAsBase64Meme;
        private readonly string _fileAsBase64WithMeme = FilesForTesting.Files.FileAsBase64WithMeme;
        private readonly string _fileAsBase64WithoutMeme = FilesForTesting.Files.FileAsBase64WithoutMeme;

        private readonly PagedRequest _validPagedRequest = new PagedRequest()
        {
            PageSize = 5,
            Page = 1,
            OrderBy = "Title",
            OrderDirection = "ASC",
            Filters = new Dictionary<string, string>()
                {
                    { "title", "Some titele" },
                    { "authors", "Some Authors" },
                    { "isbn", "111-2-33-444444-5" }
                },
            UserEmail = "test@test.com"
        };

        public BookServiceTest() 
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new BookProfile()));
            _mapper = new Mapper(configuration);

            _mockBookRepository = new Mock<IBookRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _bookService = new BookService(
                _mockBookRepository.Object,
                _mockUserRepository.Object,
                _mapper
            );
        }

        #region Create
        [Fact]
        public void Create_WithValidInput_ReturnsBookDto()
        {
            // Arrange
            var createBookDto = new CreateBookDto()
            {
                Title = "Some Title",
                Authors = "Some Authors",
                ISBN = "111-2-33-444444-5",
                Description = "Some description",
                Cover = _coverAsBase64WithMeme,
                CoverName = "Test cover",
                File = _fileAsBase64WithMeme,
                FileName = "Test file",
                AddedByEmail = "test@test.com"
            };

            _mockUserRepository.Setup(repo => repo.GetUserIdByEmail(It.IsAny<string>()))
                .ReturnsAsync(1234);
            _mockBookRepository.Setup(repo => repo.Create(It.IsAny<Book>()))
                .ReturnsAsync(new Book() 
                {
                    Id = 1,
                    Title = "Some Title",
                    Authors = "Some Authors",
                    ISBN = "111-2-33-444444-5",
                    Description = "Some description",
                    Cover = Convert.FromBase64String(_coverAsBase64WithoutMeme),
                    CoverName = "Test cover",
                    CoverMime = _coverAsBase64Meme,
                    File = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                    FileName = "Test file",
                    AddedBy = 1234
                });
            _mockBookRepository.Setup(repo => repo.GetExtra(It.IsAny<int>()))
                .ReturnsAsync(new BookExtra() 
                {
                    Id = 1,
                    Title = "Some Title",
                    Authors = "Some Authors",
                    ISBN = "111-2-33-444444-5",
                    Description = "Some description",
                    Cover = Convert.FromBase64String(_coverAsBase64WithoutMeme),
                    CoverName = "Test cover",
                    CoverMime = _coverAsBase64Meme,
                    File = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                    FileName = "Test file",
                    AddedBy = 1234,
                    AddedByUserName = "someUsernameCorespondingToUserId=1234",
                    IsInReadingList = false,
                    IsRead = false
                });

            // Act
            var result = _bookService.Create( createBookDto );

            // Assert
            Assert.IsType<Task<BookDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result.Id);
            Assert.Equal("Some Title", result.Result.Title);
            Assert.Equal("Some Authors", result.Result.Authors);
            Assert.Equal("111-2-33-444444-5", result.Result.ISBN);
            Assert.Equal("Some description", result.Result.Description);
            Assert.Equal(_coverAsBase64WithoutMeme, result.Result.Cover);
            Assert.Equal("Test cover", result.Result.CoverName);
            Assert.Equal(_coverAsBase64Meme, result.Result.CoverMime);
            Assert.Equal("Test file", result.Result.FileName);
            Assert.Equal("someUsernameCorespondingToUserId=1234", result.Result.AddedByUserName);
            Assert.False(result.Result.IsInReadingList);
            Assert.False(result.Result.IsRead);
        }
        #endregion

        #region Delete
        [Fact]
        public void Delete_WithValidInput_ReturnsBookDto()
        {
            // Arrange
            var addedBy = 1; // anonymous user have Id = 1
            var anonymousUser = "anonymous";

            var book = new Book()
            {
                Id = 1234,
                Title = "Some Title",
                Authors = "Some Authors",
                ISBN = "111-2-33-444444-5",
                Description = "Some description",
                Cover = Encoding.ASCII.GetBytes(_coverAsBase64WithoutMeme),
                CoverName = "Test cover",
                CoverMime = _coverAsBase64Meme,
                File = Encoding.ASCII.GetBytes(_fileAsBase64WithoutMeme),
                FileName = "Test file",
                AddedBy = 123
            };

            _mockUserRepository.Setup(repo => repo.GetUserIdByEmail(It.IsAny<string>()))
                .ReturnsAsync(addedBy);

            _mockBookRepository.Setup(repo => repo.Get(It.IsAny<int>()))
                .ReturnsAsync(book);

            _mockBookRepository.Setup(repo => repo.DeleteUpdate(It.IsAny<Book>()))
                .ReturnsAsync(book);

            _mockBookRepository.Setup(repo => repo.GetExtra(It.IsAny<int>()))
                .ReturnsAsync(new BookExtra() 
                { 
                    AddedBy = addedBy, 
                    AddedByUserName = anonymousUser 
                });

            // Act
            var result = _bookService.Delete(234);

            // Assert
            Assert.IsType<Task<BookDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(anonymousUser, result.Result.AddedByUserName);
            Assert.Equal(book.AddedBy, addedBy);
        }
        #endregion

        #region Get
        [Fact]
        public void Get_WithValidInput_ReturnsBookDto()
        {
            // Arrange
            var bookExtra = new BookExtra()
            {
                Id = 1234,
                Title = "Some Title",
                Authors = "Some Authors",
                ISBN = "111-2-33-444444-5",
                Description = "Some description",
                Cover = Encoding.ASCII.GetBytes(_coverAsBase64WithoutMeme),
                CoverName = "Test cover",
                CoverMime = _coverAsBase64Meme,
                File = Encoding.ASCII.GetBytes(_fileAsBase64WithoutMeme),
                FileName = "Test file",
                AddedBy = 123,
                AddedByUserName = "Some user name",
                IsInReadingList = false,
                IsRead = false,
            };

            _mockBookRepository.Setup(repo => repo.GetExtra(It.IsAny<int>()))
                .ReturnsAsync(bookExtra);

            // Act
            var result = _bookService.Get(123);

            // Assert
            Assert.IsType<Task<BookDto>>(result);
            Assert.NotNull(result.Result);
        }
        #endregion

        #region GetPagedAdminPage
        [Fact]
        public void GetPagedAdminPage_WithValidInput_ReturnsPagedResponse()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetPagedAdminPage(It.IsAny<PagedRequest>()))
                .ReturnsAsync(new PagedResponse<BookExtra>(
                    new List<BookExtra>()
                    {
                        new BookExtra()
                        {
                            Id = 1234,
                            Title = "Some Title",
                            Authors = "Some Authors",
                            ISBN = "111-2-33-444444-5",
                            Description = "Some description",
                            Cover = Convert.FromBase64String(_coverAsBase64WithoutMeme),
                            CoverName = "Test cover",
                            CoverMime = _coverAsBase64Meme,
                            File = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                            FileName = "Test file",
                            AddedBy = 123,
                            AddedByUserName = "Some user name",
                            IsInReadingList = false,
                            IsRead = false,
                        }
                    },
                    1
                ));

            // Act
            var result = _bookService.GetPagedAdminPage(_validPagedRequest);

            // Assert
            Assert.IsType<Task<PagedResponse<BookDto>>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1234, result.Result.Items[0].Id);
            Assert.Equal("Some Title", result.Result.Items[0].Title);
            Assert.Equal("Some Authors", result.Result.Items[0].Authors);
            Assert.Equal("111-2-33-444444-5", result.Result.Items[0].ISBN);
            Assert.Equal("Some description", result.Result.Items[0].Description);
            Assert.Equal(_coverAsBase64WithoutMeme, result.Result.Items[0].Cover);
            Assert.Equal("Test cover", result.Result.Items[0].CoverName);
            Assert.Equal(_coverAsBase64Meme, result.Result.Items[0].CoverMime);
            Assert.Equal("Test file", result.Result.Items[0].FileName);
            Assert.Equal("Some user name", result.Result.Items[0].AddedByUserName);
            Assert.False(result.Result.Items[0].IsInReadingList);
            Assert.False(result.Result.Items[0].IsRead);
        }
        #endregion

        #region GetPagedSearchPage
        [Fact]
        public void GetPagedSearchPage_WithValidInput_ReturnsPagedResponse()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetPagedSearchPage(It.IsAny<PagedRequest>()))
                .ReturnsAsync(new PagedResponse<BookExtra>(
                    new List<BookExtra>()
                    {
                        new BookExtra()
                        {
                            Id = 1234,
                            Title = "Some Title",
                            Authors = "Some Authors",
                            ISBN = "111-2-33-444444-5",
                            Description = "Some description",
                            Cover = Convert.FromBase64String(_coverAsBase64WithoutMeme),
                            CoverName = "Test cover",
                            CoverMime = _coverAsBase64Meme,
                            File = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                            FileName = "Test file",
                            AddedBy = 123,
                            AddedByUserName = "Some user name",
                            IsInReadingList = false,
                            IsRead = false,
                        }
                    },
                    1
                ));

            // Act
            var result = _bookService.GetPagedSearchPage(_validPagedRequest);

            // Assert
            Assert.IsType<Task<PagedResponse<BookDto>>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1234, result.Result.Items[0].Id);
            Assert.Equal("Some Title", result.Result.Items[0].Title);
            Assert.Equal("Some Authors", result.Result.Items[0].Authors);
            Assert.Equal("111-2-33-444444-5", result.Result.Items[0].ISBN);
            Assert.Equal("Some description", result.Result.Items[0].Description);
            Assert.Equal(_coverAsBase64WithoutMeme, result.Result.Items[0].Cover);
            Assert.Equal("Test cover", result.Result.Items[0].CoverName);
            Assert.Equal(_coverAsBase64Meme, result.Result.Items[0].CoverMime);
            Assert.Equal("Test file", result.Result.Items[0].FileName);
            Assert.Equal("Some user name", result.Result.Items[0].AddedByUserName);
            Assert.False(result.Result.Items[0].IsInReadingList);
            Assert.False(result.Result.Items[0].IsRead);
        }
        #endregion

        #region GetPagedReadingListPage
        [Fact]
        public void GetPagedReadingListPage_WithValidInput_ReturnsPagedResponse()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetPagedReadingListPage(It.IsAny<PagedRequest>()))
                .ReturnsAsync(new PagedResponse<BookExtra>(
                    new List<BookExtra>()
                    {
                        new BookExtra()
                        {
                            Id = 1234,
                            Title = "Some Title",
                            Authors = "Some Authors",
                            ISBN = "111-2-33-444444-5",
                            Description = "Some description",
                            Cover = Convert.FromBase64String(_coverAsBase64WithoutMeme),
                            CoverName = "Test cover",
                            CoverMime = _coverAsBase64Meme,
                            File = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                            FileName = "Test file",
                            AddedBy = 123,
                            AddedByUserName = "Some user name",
                            IsInReadingList = false,
                            IsRead = false,
                        }
                    },
                    1
                ));

            // Act
            var result = _bookService.GetPagedReadingListPage(_validPagedRequest);

            // Assert
            Assert.IsType<Task<PagedResponse<BookDto>>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(1234, result.Result.Items[0].Id);
            Assert.Equal("Some Title", result.Result.Items[0].Title);
            Assert.Equal("Some Authors", result.Result.Items[0].Authors);
            Assert.Equal("111-2-33-444444-5", result.Result.Items[0].ISBN);
            Assert.Equal("Some description", result.Result.Items[0].Description);
            Assert.Equal(_coverAsBase64WithoutMeme, result.Result.Items[0].Cover);
            Assert.Equal("Test cover", result.Result.Items[0].CoverName);
            Assert.Equal(_coverAsBase64Meme, result.Result.Items[0].CoverMime);
            Assert.Equal("Test file", result.Result.Items[0].FileName);
            Assert.Equal("Some user name", result.Result.Items[0].AddedByUserName);
            Assert.False(result.Result.Items[0].IsInReadingList);
            Assert.False(result.Result.Items[0].IsRead);
        }
        #endregion

        #region Update
        [Fact]
        public void Update_WithValidInput_ReturnsBookDto()
        {
            // Arrange
            var updatedCoverAsBase64WithMeme = "/someUpdates" + _coverAsBase64WithMeme;
            var updatedCoverAsBase64WithoutMeme = "/someUpdates" + _coverAsBase64WithoutMeme;

            var updatedFileAsBase64WithMeme = "/someUpdates" + _fileAsBase64WithMeme;
            var updatedFileAsBase64WithoutMeme = "/someUpdates" + _fileAsBase64WithoutMeme;


            var updateBookDto = new UpdateBookDto()
            {
                Id = 123,
                Title = "Updated Title",
                Authors = "Update Authors",
                ISBN = "5-4-33-222222-1",
                Description = "Updated description",
                Cover = updatedCoverAsBase64WithMeme,
                CoverName = "Updated cover",
                IsCoverEdited = true,
                File = updatedFileAsBase64WithMeme,
                FileName = "Updated file",
                IsFileEdited = true
            };
            
            _mockBookRepository.Setup(repo => repo.Update(It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new Book()
                {
                    Id = 123,
                    Title = "Updated Title",
                    Authors = "Updated Authors",
                    ISBN = "5-4-33-222222-1",
                    Description = "Updated description",
                    Cover = Convert.FromBase64String(updatedCoverAsBase64WithoutMeme),
                    CoverName = "Updated cover",
                    CoverMime = _coverAsBase64Meme,
                    File = Convert.FromBase64String(updatedFileAsBase64WithoutMeme),
                    FileName = "Updated file",
                    AddedBy = 1234
                });
            _mockBookRepository.Setup(repo => repo.GetExtra(It.IsAny<int>()))
                .ReturnsAsync(new BookExtra()
                {
                    Id = 123,
                    Title = "Updated Title",
                    Authors = "Updated Authors",
                    ISBN = "5-4-33-222222-1",
                    Description = "Updated description",
                    Cover = Convert.FromBase64String(updatedCoverAsBase64WithoutMeme),
                    CoverName = "Updated cover",
                    CoverMime = _coverAsBase64Meme,
                    File = Convert.FromBase64String(updatedFileAsBase64WithoutMeme),
                    FileName = "Updated file",
                    AddedBy = 1234,
                    AddedByUserName = "someUsernameCorespondingToUserId=1234",
                    IsInReadingList = false,
                    IsRead = false
                });

            // Act
            var result = _bookService.Update(updateBookDto);

            // Assert
            Assert.IsType<Task<BookDto>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(123, result.Result.Id);
            Assert.Equal("Updated Title", result.Result.Title);
            Assert.Equal("Updated Authors", result.Result.Authors);
            Assert.Equal("5-4-33-222222-1", result.Result.ISBN);
            Assert.Equal("Updated description", result.Result.Description);
            Assert.Equal(updatedCoverAsBase64WithoutMeme, result.Result.Cover);
            Assert.Equal("Updated cover", result.Result.CoverName);
            Assert.Equal(_coverAsBase64Meme, result.Result.CoverMime);
            Assert.Equal("Updated file", result.Result.FileName);
            Assert.Equal("someUsernameCorespondingToUserId=1234", result.Result.AddedByUserName);
            Assert.False(result.Result.IsInReadingList);
            Assert.False(result.Result.IsRead);
        }
        #endregion

        #region AddToReadingList
        [Fact]
        public void AddToReadingList_WithValidInput_ReturnsVoid()
        {
            // Arrange
            var bookToReadingListDto = new BookToReadingListDto(
                userEmail: "test@test.com",
                bookId: 123,
                isRead: false
            );

            // Act
            var result = _bookService.AddToReadingList(bookToReadingListDto);

            // Assert           
            _mockBookRepository.Verify(repo => repo.AddToReadingList(It.IsAny<BookToReadingListDto>()), Times.Once);
        }
        #endregion

        #region RemoveFromReadingList
        [Fact]
        public void RemoveFromReadingList_WithValidInput_ReturnsVoid()
        {
            // Arrange
            var bookToReadingListDto = new BookToReadingListDto(
                userEmail: "test@test.com",
                bookId: 123,
                isRead: false
            );

            // Act
            var result = _bookService.RemoveFromReadingList(bookToReadingListDto);

            // Assert           
            _mockBookRepository.Verify(repo => repo.RemoveFromReadingList(It.IsAny<BookToReadingListDto>()), Times.Once);
        }
        #endregion

        #region MarkAsReadOrUnread
        [Fact]
        public void MarkAsReadOrUnread_WithValidInput_ReturnsVoid()
        {
            // Arrange
            var bookToReadingListDto = new BookToReadingListDto(
                userEmail: "test@test.com",
                bookId: 123,
                isRead: true
            );

            // Act
            var result = _bookService.MarkAsReadOrUnread(bookToReadingListDto);

            // Assert           
            _mockBookRepository.Verify(repo => repo.MarkAsReadOrUnread(It.IsAny<BookToReadingListDto>()), Times.Once);
        }
        #endregion

        #region GetStatistics
        [Fact]
        public void GetStatistics_WithValidInput_ReturnsBookStatisticsDto()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetStatistics(It.IsAny<string>()))
                .ReturnsAsync(new BookStatisticsDto());

            // Act
            var result = _bookService.GetStatistics("test@test.com");

            // Assert
            Assert.IsType<Task<BookStatisticsDto>>(result);
            Assert.NotNull(result.Result);
        }
        #endregion

        #region GetBookForDowload
        [Fact]
        public void GetBookForDownload_WithInvalidInput_ReturnsNull()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookForDownload(It.IsAny<int>()))
                .ReturnsAsync((byte[]) null!);

            // Act
            var result = _bookService.GetBookForDownload(123);

            // Assert
            Assert.IsType<Task<string>>(result);
            Assert.Null(result.Result);
        }

        [Fact]
        public void GetBookForDownload_WithValidInput_ReturnsBase64String()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetBookForDownload(It.IsAny<int>()))
                .ReturnsAsync(Convert.FromBase64String(_fileAsBase64WithoutMeme));

            // Act
            var result = _bookService.GetBookForDownload(123);

            // Assert
            Assert.IsType<Task<string>>(result);
            Assert.NotNull(result.Result);
            Assert.Equal(_fileAsBase64WithoutMeme, result.Result);
        }
        #endregion
    }
}
