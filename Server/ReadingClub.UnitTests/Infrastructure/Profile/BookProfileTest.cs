using AutoMapper;
using ReadingClub.Domain;
using ReadingClub.Domain.Alternative;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.Profile;

namespace ReadingClub.UnitTests.Infrastructure.Profile
{
    public class BookProfileTest
    {
        private readonly IMapper _mapper;

        // small file converted in base64 string with meme (see in ReadingClub.FilesForTesting project)
        private readonly string _coverAsBase64WithMeme = FilesForTesting.Files.CoverAsBase64WithMeme;
        private readonly string _coverAsBase64WithoutMeme = FilesForTesting.Files.CoverAsBase64WithoutMeme;
        private readonly string _coverAsBase64Meme = FilesForTesting.Files.CoverAsBase64Meme;
        private readonly string _fileAsBase64WithMeme = FilesForTesting.Files.FileAsBase64WithMeme;
        private readonly string _fileAsBase64WithoutMeme = FilesForTesting.Files.FileAsBase64WithoutMeme;
        public BookProfileTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new BookProfile()));
            _mapper = new Mapper(configuration);
        }

        [Fact]
        public void BookProfile_ShouldMap_Book_To_BookDto()
        {
            // Arrange
            var book = new Book()
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
                AddedBy = 1
            };

            // Act
            var bookDto = _mapper.Map<BookDto>(book);

            // Assert
            Assert.IsType<BookDto>(bookDto);
            Assert.NotNull(bookDto);
            Assert.Equal(book.Id, bookDto.Id);
            Assert.Equal(book.Title, bookDto.Title);
            Assert.Equal(book.Authors, bookDto.Authors);
            Assert.Equal(book.ISBN, bookDto.ISBN);
            Assert.Equal(book.Description, bookDto.Description);
            Assert.Equal(Convert.ToBase64String(book.Cover), bookDto.Cover);
            Assert.Equal(book.CoverName, bookDto.CoverName);
            Assert.Equal(book.CoverMime, bookDto.CoverMime);
            Assert.Equal(book.FileName, bookDto.FileName);
        }

        [Fact]
        public void BookProfile_ShouldMap_BookExtra_To_BookDto()
        {
            // Arrange
            var book = new BookExtra()
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
                AddedBy = 1,
                AddedByUserName = "Some userName",
                IsInReadingList = true,
                IsRead = true
            };

            // Act
            var bookDto = _mapper.Map<BookDto>(book);

            // Assert
            Assert.IsType<BookDto>(bookDto);
            Assert.NotNull(bookDto);
            Assert.Equal(book.Id, bookDto.Id);
            Assert.Equal(book.Title, bookDto.Title);
            Assert.Equal(book.Authors, bookDto.Authors);
            Assert.Equal(book.ISBN, bookDto.ISBN);
            Assert.Equal(book.Description, bookDto.Description);
            Assert.Equal(Convert.ToBase64String(book.Cover), bookDto.Cover);
            Assert.Equal(book.CoverName, bookDto.CoverName);
            Assert.Equal(book.CoverMime, bookDto.CoverMime);
            Assert.Equal(book.FileName, bookDto.FileName);
            Assert.Equal(book.AddedByUserName, bookDto.AddedByUserName);
            Assert.Equal(book.IsInReadingList, bookDto.IsInReadingList);
            Assert.Equal(book.IsRead, bookDto.IsRead);
        }

        [Fact]
        public void BookProfile_ShouldMap_CreateBookDto_To_Book()
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

            // Act
            var book = _mapper.Map<Book>(createBookDto);

            // Assert
            Assert.IsType<Book>(book);
            Assert.NotNull(book);
            Assert.Equal(createBookDto.Title, book.Title);
            Assert.Equal(createBookDto.Authors, book.Authors);
            Assert.Equal(createBookDto.ISBN, book.ISBN);
            Assert.Equal(createBookDto.Description, book.Description);
            Assert.Equal(Convert.FromBase64String(_coverAsBase64WithoutMeme), book.Cover);
            Assert.Equal(createBookDto.CoverName, book.CoverName);
            Assert.Equal(_coverAsBase64Meme, book.CoverMime);
            Assert.Equal(Convert.FromBase64String(_fileAsBase64WithoutMeme), book.File);
            Assert.Equal(createBookDto.FileName, book.FileName);
        }

        [Fact]
        public void BookProfile_ShouldMap_UpdateBookDto_To_Book()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto()
            {
                Id = 1,
                Title = "Some Title",
                Authors = "Some Authors",
                ISBN = "111-2-33-444444-5",
                Description = "Some description",
                Cover = _coverAsBase64WithMeme,
                CoverName = "Test cover",
                IsCoverEdited = true,
                File = _fileAsBase64WithMeme,
                FileName = "Test file",
                IsFileEdited = true
            };

            // Act
            var book = _mapper.Map<Book>(updateBookDto);

            // Assert
            Assert.IsType<Book>(book);
            Assert.NotNull(book);
            Assert.Equal(updateBookDto.Id, book.Id);
            Assert.Equal(updateBookDto.Title, book.Title);
            Assert.Equal(updateBookDto.Authors, book.Authors);
            Assert.Equal(updateBookDto.ISBN, book.ISBN);
            Assert.Equal(updateBookDto.Description, book.Description);
            Assert.Equal(Convert.FromBase64String(_coverAsBase64WithoutMeme), book.Cover);
            Assert.Equal(updateBookDto.CoverName, book.CoverName);
            Assert.Equal(_coverAsBase64Meme, book.CoverMime);
            Assert.Equal(Convert.FromBase64String(_fileAsBase64WithoutMeme), book.File);
            Assert.Equal(updateBookDto.FileName, book.FileName);
        }
    }
}
