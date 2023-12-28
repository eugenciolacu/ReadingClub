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

        // small file converted in base64 string with meme (see in ReadingClub.UnitTests.Infrastructure.Profile)
        private readonly string _fileAsBase64WithMeme = "data:@file/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKC" +
            "gkLDRYPDQwMDRsUFRAWIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N" +
            "zc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAJQAlAMBIgACEQEDEQH/xAAcAAEAAgIDAQAAAAAAAAAAAAAABgcEBQECAwj/xAA4EAABB" +
            "AECAwUFBgUFAAAAAAABAAIDBAUGERIhMQdBUWGBEyIyQnEUIzORobEVQ1JywSVTYqLR/8QAGgEBAAMBAQEAAAAAAAAAAAAAAAIDBAUBBv/EACYRAQACA" +
            "gEDAwQDAAAAAAAAAAABAgMRBAUSMTJBkSJRcYETFCH/2gAMAwEAAhEDEQA/ALxREQEREBERARFqtUZU4TTuSybWcbqtZ8rW+JA5fqgycrk6WIoS38lYZ" +
            "Xqwt4nyPPIf+nyWg0/2h6bz95lGhckbYlBMLJ4XR+1268JI2P06qjsxlMvlH4dmcytm5FZnE0kMhaIg/h93hAA2+IhZWWc6Oi+zGQ2aptZhf/Q9h3BH5" +
            "KqcsRMR91c5IidPpELlVjpntCyMmVq0s3FWfBbeI2WYGFhjefhDmkncE8tx0Oys0KytomNwnExPhyiIvXoiIgIiICIiAiIgIiICIiAsPL0I8pi7lCb8O" +
            "zC+J3kHDZZiIPlrMULtWCXB5KF1fL4zaSLfpM1nRzD37j9vyyZrbclh4PZfFeLYuHrtv8f5AOV19pumYc7p6axDEP4rj2OsU5Wj3+Jo34N+8O2226dFR" +
            "Wm6FjI5JtSm50TLc0bK5H8kT8Je4f2tB29VTbHHlXaiZaMxc2odSVW1BvQx07Zrlj5eJvNsYPQu32J8AFeYWBgsRRweMgx2NhbDWhbs0DqT3uJ7yepK2" +
            "CspWKxqE6xqNCIik9EREBERAREQEREBERARF1J270HJOyh+qu0XA6cmNR8kl3IdPsdMcbwfM9G+p38lEdY65u6gsz4jS1g1sfE72drJs+KQ97Yv24vy8" +
            "4/jcbVxkBiqR8O53c8ndzj4kqu+SKujw+m5eT9Xiv3ba92ha0yRIx2Lx+Kru5A2iZZNv2HqFEMdiM/jp2WaeWrRTxuDmfccQa4DYEbhb+SeCOQMkmiY9" +
            "3RrngEr0e5rGOc8hrWjdzncgAqJzWl2KdI4kR9U7/bvBrftCxzg6X+G5WIdWez4HEeRG3+VLtLdq+HytptDMQyYbIEgCOyfu3HuAfy29QPVQarfp23OZ" +
            "VtQyuHUMeCV1yWNqZOAw3Yg9vc7o5v0PcpRmmPVCjL0bHevdgsv4EHouVTPZ1q+7p7J19MahsGejYIZjrr+rHf7bv0A8Pp0uUdFoiduBkx2x2mto1MOU" +
            "RF6gIiICIiAiIgIiICrftZ1DOwQ6YxUpjt32F1qZp5wVxyPq7mB6qxyqElsuyWrNR5KUu4zddVaD8rIvdAH5bqF7dtdtfCwRnz1pPj3d6teGpXjr12Bk" +
            "UY2a0dwXr3rhFi3v/X2sViK9sNV2fnBYPMOz2rsnUdYlllqmm+IvfC87ESuHP3dg4b7bDiHp21RBWz7oW4yx9hxFm8D7aVhAjhO4Dy3rw78wOWwI3225" +
            "ZdmpVlPt5qkU0kbS5pdGHO5DfYErDp5+lbiqeydvPOQ012nd0Z+bfwA8Vo/k3rUODHT64bXrfJ6vHy29LV2DdpKtpOnA6fI05RC2WGP7o8D/enDvBwBO" +
            "3U8Wy6LrHFHFxCKNjN+vC0DddlVkv3z4dHgcP8Aq0mszvbEy9CPJ0JasnIu5sd3scOhVr9m+el1FpKlbtb/AGuPeCzv19ow7E+vI+qrRS3sTH+mZ17Pw" +
            "n5aQsPc73GAkeqtwT5hzOuYqxNb+/hY6Ii0PnxERAREQEREBERAKovU2PfprWt+CdvBRy0v2qnJ8pefxGE+O/P8vFXotVqLA47UeNkx+VriWF3MEcnMP" +
            "9TT3FRtWLRqV3Hz2wZYyV9lP7jcDcc+i2dLAZa7sYKE4afmkbwD/tstzXwmsNJSH+FR0NRUR8LbQENwDuHtNtnfU/os6LtKp1nmPUWEzOFc3k6SzULof" +
            "R7d+XntsqYwR7y7GTrt59FNflrq+hMtIN5pakXkHucf2/yvWLs0dFI+aOzUjlf8b2V9i76lTnE5XHZit9pxV2vbh6ccMgcAfA7dD5FZvJWRirDFbqvJt" +
            "O9x8QribQOTaPuLVSQ+Di5v+CtVc0zmaY3fSfI0dXQfefoOf6KwtSaqw2m42HJ2w2WT8KvGOOWX+1g5n69FE7Ga1vqwex09iH6foycnX8o3afh/4Rjof" +
            "r49R1Xk4aysp1nk19WpQa4b1nIx4LDwOfmLHLgkaWiu3bm+TvaADuPTy3ujSWAraawFTFVDxMgb7zz1keebnH6lYmjdIY/StWUVzLZuWDxWbtk8Usx8z" +
            "4eSkanSkVjUMnL5d+TfusIiKTKIiICIiAiIgIiICIiAuj42vBDwHNPUEbhd0QQfU+kI6LJ8/pJjcbma7DKWQDhiuBoJMcjBsDv49QVKNP5OPN4OhlIml" +
            "rLcDJeA/LxDcj06KOa4z8z3jS2A+9zmQjIJb0pwnk6V57tt+Q7zsvVubx2m6dTTuJjmymQq12Qx06g4nDhbsDI74YwduriPVB49n+KgsRWtRXg2xmLtm" +
            "Zskz+ZgayRzBEzwDeHbl1Kmi02ksZYxWGbDdcw2pZpbM/s/ha+WRz3Nb5Au238luUBERAREQEREBERAREQEREBERAXnZe6OvLIxvE9rCWt8SB0XoiCrN" +
            "I7jsuv6hgsudlck2Se9cZ70jPeIdt124Gb7Du26KwsHjMdisdDXxEMcVXYOaWc+Pf5ierieu55lQ7I4fKaLvW8xpmv9txFl5lyGIHJzXH4pIfPb5e/8t" +
            "tlpzKUI6NO5hZva4C3I2JsPQ0ZHHYMA7m8RDeH5SRt7vQJgiIgIiICIiAiIgIiICIiAiIgIiICIiDgqqqrWx9o2d05io/uLc9S7O1rfchLTxyu8i7Zg+" +
            "rvJTnWuooNLadtZaw3j9kOGKPfbjkPJrd+4b9T3DdaTsqxFythp81mW/wCq5qY3J9xsWNPwM58wAO7u327kE4REQEREBERAREQEREBERAREQEREBERBW" +
            "Or4DqrtRw+nZueOxtf+JWYz/MdxcIBHq30cVZo5DZRGzibtLtJr5yvC6eleoGlZ4dvuHtPG158iAW8u9S4dEHKIiAiIgIiICIiAiIgIiICIiAiIgIiIO" +
            "CuURAREQEREBERAREQEREH/2Q==";
        private readonly string _fileAsBase64WithoutMeme =  "/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKCgkLDRYPDQwMDRsUFRAW" +
            "IB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3" +
            "Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAJQAlAMBIgACEQEDEQH/xAAcAAEAAgIDAQAAAAAAAAAAAAAABgcEBQECAwj/xAA4EAABBAECAwUFBgUFAAAAAAAB" +
            "AAIDBAUGERIhMQdBUWGBEyIyQnEUIzORobEVQ1JywSVTYqLR/8QAGgEBAAMBAQEAAAAAAAAAAAAAAAIDBAUBBv/EACYRAQACAgEDAwQDAAAAAAAAAAAB" +
            "AgMRBAUSMTJBkSJRcYETFCH/2gAMAwEAAhEDEQA/ALxREQEREBERARFqtUZU4TTuSybWcbqtZ8rW+JA5fqgycrk6WIoS38lYZXqwt4nyPPIf+nyWg0/2" +
            "h6bz95lGhckbYlBMLJ4XR+1268JI2P06qjsxlMvlH4dmcytm5FZnE0kMhaIg/h93hAA2+IhZWWc6Oi+zGQ2aptZhf/Q9h3BH5KqcsRMR91c5IidPpELl" +
            "VjpntCyMmVq0s3FWfBbeI2WYGFhjefhDmkncE8tx0Oys0KytomNwnExPhyiIvXoiIgIiICIiAiIgIiICIiAsPL0I8pi7lCb8OzC+J3kHDZZiIPlrMULt" +
            "WCXB5KF1fL4zaSLfpM1nRzD37j9vyyZrbclh4PZfFeLYuHrtv8f5AOV19pumYc7p6axDEP4rj2OsU5Wj3+Jo34N+8O2226dFRWm6FjI5JtSm50TLc0bK" +
            "5H8kT8Je4f2tB29VTbHHlXaiZaMxc2odSVW1BvQx07Zrlj5eJvNsYPQu32J8AFeYWBgsRRweMgx2NhbDWhbs0DqT3uJ7yepK2CspWKxqE6xqNCIik9ER" +
            "EBERAREQEREBERARF1J270HJOyh+qu0XA6cmNR8kl3IdPsdMcbwfM9G+p38lEdY65u6gsz4jS1g1sfE72drJs+KQ97Yv24vy84/jcbVxkBiqR8O53c8n" +
            "dzj4kqu+SKujw+m5eT9Xiv3ba92ha0yRIx2Lx+Kru5A2iZZNv2HqFEMdiM/jp2WaeWrRTxuDmfccQa4DYEbhb+SeCOQMkmiY93RrngEr0e5rGOc8hrWj" +
            "dzncgAqJzWl2KdI4kR9U7/bvBrftCxzg6X+G5WIdWez4HEeRG3+VLtLdq+HytptDMQyYbIEgCOyfu3HuAfy29QPVQarfp23OZVtQyuHUMeCV1yWNqZOA" +
            "w3Yg9vc7o5v0PcpRmmPVCjL0bHevdgsv4EHouVTPZ1q+7p7J19MahsGejYIZjrr+rHf7bv0A8Pp0uUdFoiduBkx2x2mto1MOURF6gIiICIiAiIgIiICr" +
            "ftZ1DOwQ6YxUpjt32F1qZp5wVxyPq7mB6qxyqElsuyWrNR5KUu4zddVaD8rIvdAH5bqF7dtdtfCwRnz1pPj3d6teGpXjr12BkUY2a0dwXr3rhFi3v/X2" +
            "sViK9sNV2fnBYPMOz2rsnUdYlllqmm+IvfC87ESuHP3dg4b7bDiHp21RBWz7oW4yx9hxFm8D7aVhAjhO4Dy3rw78wOWwI3225ZdmpVlPt5qkU0kbS5pd" +
            "GHO5DfYErDp5+lbiqeydvPOQ012nd0Z+bfwA8Vo/k3rUODHT64bXrfJ6vHy29LV2DdpKtpOnA6fI05RC2WGP7o8D/enDvBwBO3U8Wy6LrHFHFxCKNjN+" +
            "vC0DddlVkv3z4dHgcP8Aq0mszvbEy9CPJ0JasnIu5sd3scOhVr9m+el1FpKlbtb/AGuPeCzv19ow7E+vI+qrRS3sTH+mZ17Pwn5aQsPc73GAkeqtwT5h" +
            "zOuYqxNb+/hY6Ii0PnxERAREQEREBERAKovU2PfprWt+CdvBRy0v2qnJ8pefxGE+O/P8vFXotVqLA47UeNkx+VriWF3MEcnMP9TT3FRtWLRqV3Hz2wZY" +
            "yV9lP7jcDcc+i2dLAZa7sYKE4afmkbwD/tstzXwmsNJSH+FR0NRUR8LbQENwDuHtNtnfU/os6LtKp1nmPUWEzOFc3k6SzULofR7d+XntsqYwR7y7GTrt" +
            "59FNflrq+hMtIN5pakXkHucf2/yvWLs0dFI+aOzUjlf8b2V9i76lTnE5XHZit9pxV2vbh6ccMgcAfA7dD5FZvJWRirDFbqvJtO9x8QribQOTaPuLVSQ+" +
            "Di5v+CtVc0zmaY3fSfI0dXQfefoOf6KwtSaqw2m42HJ2w2WT8KvGOOWX+1g5n69FE7Ga1vqwex09iH6foycnX8o3afh/4Rjofr49R1Xk4aysp1nk19Wp" +
            "Qa4b1nIx4LDwOfmLHLgkaWiu3bm+TvaADuPTy3ujSWAraawFTFVDxMgb7zz1keebnH6lYmjdIY/StWUVzLZuWDxWbtk8Usx8z4eSkanSkVjUMnL5d+Tf" +
            "usIiKTKIiICIiAiIgIiICIiAuj42vBDwHNPUEbhd0QQfU+kI6LJ8/pJjcbma7DKWQDhiuBoJMcjBsDv49QVKNP5OPN4OhlImlrLcDJeA/LxDcj06KOa4" +
            "z8z3jS2A+9zmQjIJb0pwnk6V57tt+Q7zsvVubx2m6dTTuJjmymQq12Qx06g4nDhbsDI74YwduriPVB49n+KgsRWtRXg2xmLtmZskz+ZgayRzBEzwDeHb" +
            "l1Kmi02ksZYxWGbDdcw2pZpbM/s/ha+WRz3Nb5Au238luUBERAREQEREBERAREQEREBERAXnZe6OvLIxvE9rCWt8SB0XoiCrNI7jsuv6hgsudlck2Se9" +
            "cZ70jPeIdt124Gb7Du26KwsHjMdisdDXxEMcVXYOaWc+Pf5ierieu55lQ7I4fKaLvW8xpmv9txFl5lyGIHJzXH4pIfPb5e/8ttlpzKUI6NO5hZva4C3I" +
            "2JsPQ0ZHHYMA7m8RDeH5SRt7vQJgiIgIiICIiAiIgIiICIiAiIgIiICIiDgqqqrWx9o2d05io/uLc9S7O1rfchLTxyu8i7Zg+rvJTnWuooNLadtZaw3j" +
            "9kOGKPfbjkPJrd+4b9T3DdaTsqxFythp81mW/wCq5qY3J9xsWNPwM58wAO7u327kE4REQEREBERAREQEREBERAREQEREBERBWOr4DqrtRw+nZueOxtf+" +
            "JWYz/MdxcIBHq30cVZo5DZRGzibtLtJr5yvC6eleoGlZ4dvuHtPG158iAW8u9S4dEHKIiAiIgIiICIiAiIgIiICIiAiIgIiIOCuURAREQEREBERAREQE" +
            "REH/2Q==";
        private readonly string _fileAsBase64Meme = "data:@file/jpeg;base64";

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
                Cover = _fileAsBase64WithMeme,
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
                    Cover = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                    CoverName = "Test cover",
                    CoverMime = _fileAsBase64Meme,
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
                    Cover = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                    CoverName = "Test cover",
                    CoverMime = _fileAsBase64Meme,
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
            Assert.Equal(_fileAsBase64WithoutMeme, result.Result.Cover);
            Assert.Equal("Test cover", result.Result.CoverName);
            Assert.Equal(_fileAsBase64Meme, result.Result.CoverMime);
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
                Cover = Encoding.ASCII.GetBytes(_fileAsBase64WithoutMeme),
                CoverName = "Test cover",
                CoverMime = _fileAsBase64Meme,
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
                Cover = Encoding.ASCII.GetBytes(_fileAsBase64WithoutMeme),
                CoverName = "Test cover",
                CoverMime = _fileAsBase64Meme,
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
                            Cover = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                            CoverName = "Test cover",
                            CoverMime = _fileAsBase64Meme,
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
            Assert.Equal(_fileAsBase64WithoutMeme, result.Result.Items[0].Cover);
            Assert.Equal("Test cover", result.Result.Items[0].CoverName);
            Assert.Equal(_fileAsBase64Meme, result.Result.Items[0].CoverMime);
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
                            Cover = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                            CoverName = "Test cover",
                            CoverMime = _fileAsBase64Meme,
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
            Assert.Equal(_fileAsBase64WithoutMeme, result.Result.Items[0].Cover);
            Assert.Equal("Test cover", result.Result.Items[0].CoverName);
            Assert.Equal(_fileAsBase64Meme, result.Result.Items[0].CoverMime);
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
                            Cover = Convert.FromBase64String(_fileAsBase64WithoutMeme),
                            CoverName = "Test cover",
                            CoverMime = _fileAsBase64Meme,
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
            Assert.Equal(_fileAsBase64WithoutMeme, result.Result.Items[0].Cover);
            Assert.Equal("Test cover", result.Result.Items[0].CoverName);
            Assert.Equal(_fileAsBase64Meme, result.Result.Items[0].CoverMime);
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
            var updatedFileAsBase64WithMeme = "/someUpdates" + _fileAsBase64WithMeme;
            var updatedFileAsBase64WithoutMeme = "/someUpdates" + _fileAsBase64WithoutMeme;

            var updateBookDto = new UpdateBookDto()
            {
                Id = 123,
                Title = "Updated Title",
                Authors = "Update Authors",
                ISBN = "5-4-33-222222-1",
                Description = "Updated description",
                Cover = updatedFileAsBase64WithMeme,
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
                    Cover = Convert.FromBase64String(updatedFileAsBase64WithoutMeme),
                    CoverName = "Updated cover",
                    CoverMime = _fileAsBase64Meme,
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
                    Cover = Convert.FromBase64String(updatedFileAsBase64WithoutMeme),
                    CoverName = "Updated cover",
                    CoverMime = _fileAsBase64Meme,
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
            Assert.Equal(updatedFileAsBase64WithoutMeme, result.Result.Cover);
            Assert.Equal("Updated cover", result.Result.CoverName);
            Assert.Equal(_fileAsBase64Meme, result.Result.CoverMime);
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

            //_mockBookRepository.Setup(repo => repo.AddToReadingList(It.IsAny<BookToReadingListDto>()));

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

            //_mockBookRepository.Setup(repo => repo.RemoveFromReadingList(It.IsAny<BookToReadingListDto>()));

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

            //_mockBookRepository.Setup(repo => repo.MarkAsReadOrUnread(It.IsAny<BookToReadingListDto>()));

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
