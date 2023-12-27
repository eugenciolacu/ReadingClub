using AutoMapper;
using ReadingClub.Domain;
using ReadingClub.Domain.Alternative;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.Profile;
using System.Text;

namespace ReadingClub.UnitTests.Infrastructure.Profile
{
    public class BookProfileTest
    {
        private readonly IMapper _mapper;
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
                Cover = Convert.FromBase64String("test"),
                CoverName = "Test cover",
                CoverMime = "data:image/png;base64",
                File = Convert.FromBase64String("SomeFile"),
                FileName = "Test file",
                AddedBy = 1
            };

            // Act
            var bookDto = _mapper.Map<BookDto>(book);

            // Assert
            Assert.IsType<BookDto>(bookDto);
            Assert.NotNull(bookDto);
            Assert.Equal(bookDto.Id, book.Id);
            Assert.Equal(bookDto.Title, book.Title);
            Assert.Equal(bookDto.Authors, book.Authors);
            Assert.Equal(bookDto.ISBN, book.ISBN);
            Assert.Equal(bookDto.Description, book.Description);
            Assert.Equal(bookDto.Cover, Convert.ToBase64String(book.Cover));
            Assert.Equal(bookDto.CoverName, book.CoverName);
            Assert.Equal(bookDto.CoverMime, book.CoverMime);
            Assert.Equal(bookDto.FileName, book.FileName);
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
                Cover = Convert.FromBase64String("test"),
                CoverName = "Test cover",
                CoverMime = "data:image/png;base64",
                File = Convert.FromBase64String("SomeFile"),
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
            Assert.Equal(bookDto.Id, book.Id);
            Assert.Equal(bookDto.Title, book.Title);
            Assert.Equal(bookDto.Authors, book.Authors);
            Assert.Equal(bookDto.ISBN, book.ISBN);
            Assert.Equal(bookDto.Description, book.Description);
            Assert.Equal(bookDto.Cover, Convert.ToBase64String(book.Cover));
            Assert.Equal(bookDto.CoverName, book.CoverName);
            Assert.Equal(bookDto.CoverMime, book.CoverMime);
            Assert.Equal(bookDto.FileName, book.FileName);
            Assert.Equal(bookDto.AddedByUserName, book.AddedByUserName);
            Assert.Equal(bookDto.IsInReadingList, book.IsInReadingList);
            Assert.Equal(bookDto.IsRead, book.IsRead);
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
                // small file converted in base64 string with meme
                Cover = "data:@file/jpeg;base64,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",
                CoverName = "Test cover",
                // small file converted in base64 string with meme
                File = "data:@file/jpeg;base64,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",
                FileName = "Test file",
                AddedByEmail = "test@test.com"
            };

            // Act
            var book = _mapper.Map<Book>(createBookDto);

            // Assert
            Assert.IsType<Book>(book);
            Assert.NotNull(book);
            Assert.Equal(book.Title, createBookDto.Title);
            Assert.Equal(book.Authors, createBookDto.Authors);
            Assert.Equal(book.ISBN, createBookDto.ISBN);
            Assert.Equal(book.Description, createBookDto.Description);
            Assert.Equal(book.Cover, Convert.FromBase64String("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"));
            Assert.Equal(book.CoverName, createBookDto.CoverName);
            Assert.Equal(book.CoverMime, "data:@file/jpeg;base64");
            Assert.Equal(book.File, Convert.FromBase64String("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"));
            Assert.Equal(book.FileName, createBookDto.FileName);
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
                // small file converted in base64 string with meme
                Cover = "data:@file/jpeg;base64,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",
                CoverName = "Test cover",
                IsCoverEdited = true,
                // small file converted in base64 string with meme
                File = "data:@file/jpeg;base64,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",
                FileName = "Test file",
                IsFileEdited = true
            };

            // Act
            var book = _mapper.Map<Book>(updateBookDto);

            // Assert
            Assert.IsType<Book>(book);
            Assert.NotNull(book);
            Assert.Equal(book.Id, updateBookDto.Id);
            Assert.Equal(book.Title, updateBookDto.Title);
            Assert.Equal(book.Authors, updateBookDto.Authors);
            Assert.Equal(book.ISBN, updateBookDto.ISBN);
            Assert.Equal(book.Description, updateBookDto.Description);
            Assert.Equal(book.Cover, Convert.FromBase64String("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"));
            Assert.Equal(book.CoverName, updateBookDto.CoverName);
            Assert.Equal(book.CoverMime, "data:@file/jpeg;base64");
            Assert.Equal(book.File, Convert.FromBase64String("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"));
            Assert.Equal(book.FileName, updateBookDto.FileName);
        }
    }
}
