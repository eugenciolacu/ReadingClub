using Newtonsoft.Json;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.DTO.User;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ReadingClub.IntegrationTest.Controllers
{
    public class BookControllerTest : IDisposable
    {
        private CustomWebApplicationFactory _customWebApplicationFactory;
        private HttpClient _httpClient;

        public BookControllerTest()
        {
            _customWebApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _customWebApplicationFactory.CreateClient();
        }

        #region GetPagedAdminPage
        [Fact]
        public async Task GetPagedAdminPage_WithValidInput_PagedResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();
            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var books = new List<Book>();
            List<Task<Book>> tasks = new List<Task<Book>>();
            tasks.Add(TestHelper.AddNewBookToTestDatabase(user.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabase(user.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabase(user.Id));
            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
            // Retrieve the results from the completed tasks and add them to the books list
            books.AddRange(tasks.Select(task => task.Result));
            books = books.OrderBy(x => x.Title).ToList();

            PagedRequest validPagedRequest = new PagedRequest(5, 1, "title", "Ascending",
                new Dictionary<string, string>() {
                    { "title", "" },
                    { "authors", "" },
                    { "isbn", "" }
                },
                user.Email);

            string json = JsonConvert.SerializeObject(validPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedAdminPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (PagedResponse<BookDto>)null! });

            Assert.True(resultContent?.Status);
            Assert.IsType<PagedResponse<BookDto>>(resultContent?.Data);
            Assert.Equal(3, resultContent.Data.TotalItems);
            Assert.Equal(3, resultContent.Data.Items.Count);
            Assert.Distinct(resultContent.Data.Items);
            Assert.Equal(books[0].Title, resultContent.Data.Items[0].Title);
            Assert.Equal(books[1].Title, resultContent.Data.Items[1].Title);
            Assert.Equal(books[2].Title, resultContent.Data.Items[2].Title);
        }

        [Fact]
        public async Task GetPagedAdminPage_WithInvalidInput_ReturnsErrorHandlingMiddlewareErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();
            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            List<Task<Book>> tasks = new List<Task<Book>>();
            tasks.Add(TestHelper.AddNewBookToTestDatabase(user.Id));
            await Task.WhenAll(tasks);

            PagedRequest invalidPagedRequest = new PagedRequest();

            string json = JsonConvert.SerializeObject(invalidPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedAdminPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { status = false, message = (string)null! });

            Assert.False(resultContent?.status);
            Assert.Equal("An unexpected error occurred during processing.", resultContent?.message);
        }
        #endregion

        #region GetPagedSearchPage
        [Fact]
        public async Task GetPagedSearchPage_WithValidInput_PagedResponse()
        {
            // Arrange
            var books = new List<Book>();
            List<Task<Book>> tasks = new List<Task<Book>>();

            string someGuidAsSpecificAuthor = Guid.NewGuid().ToString();

            User user1 = await TestHelper.AddNewUserToTestDatabase();            
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user1.Id, someGuidAsSpecificAuthor));

            User user2 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user2.Id, someGuidAsSpecificAuthor));

            User user3 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user3.Id, someGuidAsSpecificAuthor));

            User user4 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user4.Id, someGuidAsSpecificAuthor));

            User user5 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user5.Id, someGuidAsSpecificAuthor));

            User user6 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user6.Id, someGuidAsSpecificAuthor));

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
            // Retrieve the results from the completed tasks and add them to the books list
            books.AddRange(tasks.Select(task => task.Result));
            books = books.OrderByDescending(x => x.ISBN).ToList();

            var testUser = await TestHelper.AddNewUserToTestDatabase();
            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = testUser.UserName,
                Email = testUser.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PagedRequest validPagedRequest = new PagedRequest(5, 1, "isbn", "Descending",
                new Dictionary<string, string>() {
                    { "title", "" },
                    { "authors", "Authors" + someGuidAsSpecificAuthor },
                    { "isbn", "" }
                },
                testUser.Email);

            string json = JsonConvert.SerializeObject(validPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedSearchPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (PagedResponse<BookDto>)null! });

            Assert.True(resultContent?.Status);
            Assert.IsType<PagedResponse<BookDto>>(resultContent?.Data);
            Assert.Equal(6, resultContent.Data.TotalItems);
            Assert.Equal(5, resultContent.Data.Items.Count);
            Assert.Distinct(resultContent.Data.Items);
            Assert.Equal(books[0].ISBN, resultContent.Data.Items[0].ISBN);
            Assert.Equal(books[1].ISBN, resultContent.Data.Items[1].ISBN);
            Assert.Equal(books[2].ISBN, resultContent.Data.Items[2].ISBN);
            Assert.Equal(books[3].ISBN, resultContent.Data.Items[3].ISBN);
            Assert.Equal(books[4].ISBN, resultContent.Data.Items[4].ISBN);
            Assert.True(testUser.UserName != resultContent.Data.Items[0].AddedByUserName);
            Assert.True(testUser.UserName != resultContent.Data.Items[1].AddedByUserName);
            Assert.True(testUser.UserName != resultContent.Data.Items[2].AddedByUserName);
            Assert.True(testUser.UserName != resultContent.Data.Items[3].AddedByUserName);
            Assert.True(testUser.UserName != resultContent.Data.Items[4].AddedByUserName);
            Assert.True(resultContent.Data.Items[0].Authors == "Authors" + someGuidAsSpecificAuthor);
            Assert.True(resultContent.Data.Items[1].Authors == "Authors" + someGuidAsSpecificAuthor);
            Assert.True(resultContent.Data.Items[2].Authors == "Authors" + someGuidAsSpecificAuthor);
            Assert.True(resultContent.Data.Items[3].Authors == "Authors" + someGuidAsSpecificAuthor);
            Assert.True(resultContent.Data.Items[4].Authors == "Authors" + someGuidAsSpecificAuthor);
        }

        [Fact]
        public async Task GetPagedSearchPage_WithInvalidInput_ReturnsErrorHandlingMiddlewareErrorResponse()
        {
            // Arrange
            var books = new List<Book>();
            List<Task<Book>> tasks = new List<Task<Book>>();

            string someGuidAsSpecificAuthor = Guid.NewGuid().ToString();

            User user1 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookOfSpecificAuthorToTestDatabase(user1.Id, someGuidAsSpecificAuthor));
            await Task.WhenAll(tasks);

            var testUser = await TestHelper.AddNewUserToTestDatabase();
            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = testUser.UserName,
                Email = testUser.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PagedRequest invalidPagedRequest = new PagedRequest();

            string json = JsonConvert.SerializeObject(invalidPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedSearchPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { status = false, message = (string)null! });

            Assert.False(resultContent?.status);
            Assert.Equal("An unexpected error occurred during processing.", resultContent?.message);
        }
        #endregion

        #region GetPagedReadingListPage
        [Fact]
        public async Task GetPagedReadingListPage_WithValidInput_PagedResponse()
        {
            // Arrange
            User specificUser = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = specificUser.UserName,
                Email = specificUser.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var books = new List<Book>();
            List<Task<Book>> tasks = new List<Task<Book>>();

            User user1 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user1.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user1.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user1.Id, specificUser.Id));

            User user2 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user2.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user2.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user2.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user2.Id, specificUser.Id));

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
            // Retrieve the results from the completed tasks and add them to the books list
            books.AddRange(tasks.Select(task => task.Result));
            books = books.OrderBy(x => x.Authors).ToList();

            PagedRequest validPagedRequest = new PagedRequest(5, 2, "Authors", "Ascending",
                new Dictionary<string, string>() {
                    { "title", "" },
                    { "authors", "" },
                    { "isbn", "" },
                    { "isReadFilter", "" }
                },
                specificUser.Email);

            string json = JsonConvert.SerializeObject(validPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedReadingListPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (PagedResponse<BookDto>)null! });

            Assert.True(resultContent?.Status);
            Assert.IsType<PagedResponse<BookDto>>(resultContent?.Data);
            Assert.Equal(7, resultContent.Data.TotalItems);
            Assert.Equal(2, resultContent.Data.Items.Count);
            Assert.Distinct(resultContent.Data.Items);
            // order ascending, 5 items per page, 2 page
            Assert.Equal(books[5].Authors, resultContent.Data.Items[0].Authors);
            Assert.Equal(books[6].Authors, resultContent.Data.Items[1].Authors);
            Assert.False(resultContent.Data.Items[0].IsRead);
            Assert.False(resultContent.Data.Items[1].IsRead);
        }

        [Fact]
        public async Task GetPagedReadingListPage_WithInvalidInput_ReturnsErrorHandlingMiddlewareErrorResponse()
        {
            User specificUser = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = specificUser.UserName,
                Email = specificUser.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var books = new List<Book>();
            List<Task<Book>> tasks = new List<Task<Book>>();

            User user1 = await TestHelper.AddNewUserToTestDatabase();
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user1.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user1.Id, specificUser.Id));
            tasks.Add(TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user1.Id, specificUser.Id));

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
            // Retrieve the results from the completed tasks and add them to the books list
            books.AddRange(tasks.Select(task => task.Result));
            books = books.OrderBy(x => x.Authors).ToList();

            PagedRequest invalidPagedRequest = new PagedRequest();

            string json = JsonConvert.SerializeObject(invalidPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedReadingListPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { status = false, message = (string)null! });

            Assert.False(resultContent?.status);
            Assert.Equal("An unexpected error occurred during processing.", resultContent?.message);
        }
        #endregion

        #region Create
        [Fact]
        public async Task Create_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            CreateBookDto createBookDto = new CreateBookDto();

            string json = JsonConvert.SerializeObject(createBookDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(5, resultContent?.errors.Count);
            Assert.Equal("The Title field is required.", resultContent?.errors["Title"][0]);
            Assert.Equal("The Authors field is required.", resultContent?.errors["Authors"][0]);
            Assert.Equal("The File field is required.", resultContent?.errors["File"][0]);
            Assert.Equal("The FileName field is required.", resultContent?.errors["FileName"][0]);
            Assert.Equal("The AddedByEmail field is required.", resultContent?.errors["AddedByEmail"][0]);
        }

        [Fact]
        public async Task Create_WithValidInput_ReturnsBookDto()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string someGuidAsRandomValue = Guid.NewGuid().ToString();

            CreateBookDto createBookDto = new CreateBookDto()
            {
                Title = "Title " + someGuidAsRandomValue,
                Authors = "Authors " + someGuidAsRandomValue,
                ISBN = "ISBN " + someGuidAsRandomValue,
                Description = "Description " + someGuidAsRandomValue,
                Cover = FilesForTesting.Files.CoverAsBase64WithMeme,
                CoverName = "CoverName " + someGuidAsRandomValue,
                File = FilesForTesting.Files.FileAsBase64WithMeme,
                FileName = "FileName " + someGuidAsRandomValue,
                AddedByEmail = user.Email
            };

            string json = JsonConvert.SerializeObject(createBookDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (BookDto)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(createBookDto.Title, resultContent?.Data.Title);
            Assert.Equal(createBookDto.Authors, resultContent?.Data.Authors);
            Assert.Equal(createBookDto.ISBN, resultContent?.Data.ISBN);
            Assert.Equal(createBookDto.Description, resultContent?.Data.Description);
            Assert.Equal(FilesForTesting.Files.CoverAsBase64WithoutMeme, resultContent?.Data.Cover);
            Assert.Equal(FilesForTesting.Files.CoverAsBase64Meme, resultContent?.Data.CoverMime);
            Assert.Equal(createBookDto.CoverName, resultContent?.Data.CoverName);
            Assert.Equal(createBookDto.FileName, resultContent?.Data.FileName);
            Assert.Equal(user.UserName, resultContent?.Data.AddedByUserName);
            Assert.False(resultContent?.Data.IsInReadingList);
            Assert.False(resultContent?.Data.IsRead);
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            UpdateBookDto updateBookDto = new UpdateBookDto();

            string json = JsonConvert.SerializeObject(updateBookDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/Book/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(4, resultContent?.errors.Count);
            Assert.Equal("The Title field is required.", resultContent?.errors["Title"][0]);
            Assert.Equal("The Authors field is required.", resultContent?.errors["Authors"][0]);
            Assert.Equal("The File field is required.", resultContent?.errors["File"][0]);
            Assert.Equal("The FileName field is required.", resultContent?.errors["FileName"][0]);
        }

        [Fact]
        public async Task Update_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);

            UpdateBookDto updateBookDto = new UpdateBookDto()
            {
                Id = -1,
                Title = book.Title,
                Authors = book.Authors,
                ISBN = book.ISBN,
                Description = book.Description,
                Cover = Convert.ToBase64String(book.Cover!),
                CoverName = book.CoverName,
                IsCoverEdited = true,
                File = Convert.ToBase64String(book.File!),
                FileName = book.FileName,
                IsFileEdited = true
            };

            string json = JsonConvert.SerializeObject(updateBookDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/Book/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, book not found.", resultContent?.Message);
        }

        [Fact]
        public async Task Update_WithValidInput_ReturnsBookDto()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);

            UpdateBookDto updateBookDto = new UpdateBookDto()
            {
                Id = book.Id,
                Title = "Updated " + book.Title,
                Authors = "Updated " + book.Authors,
                ISBN = "Updated " + book.ISBN,
                Description = "Updated " + book.Description,
                Cover = FilesForTesting.Files.CoverAsBase64WithMeme,
                CoverName = "Updated " + book.CoverName,
                IsCoverEdited = true,
                File = FilesForTesting.Files.FileAsBase64WithMeme,
                FileName = "Updated " + book.FileName,
                IsFileEdited = true
            };

            string json = JsonConvert.SerializeObject(updateBookDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/Book/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (BookDto)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(updateBookDto.Id, resultContent?.Data.Id);
            Assert.Equal(updateBookDto.Title, resultContent?.Data.Title);
            Assert.Equal(updateBookDto.Authors, resultContent?.Data.Authors);
            Assert.Equal(updateBookDto.ISBN, resultContent?.Data.ISBN);
            Assert.Equal(updateBookDto.Description, resultContent?.Data.Description);
            Assert.Equal(updateBookDto.CoverName, resultContent?.Data.CoverName);
            Assert.Equal(updateBookDto.FileName, resultContent?.Data.FileName);
            Assert.Equal(user.UserName, resultContent?.Data.AddedByUserName);

            // in front-end cover is computed as << this.book.coverMime + ',' + this.book.cover >>
            // so updateBookDto.Cover will contain cover and ',' symbol
            // so these have to be removed before assert
            int startIndex = updateBookDto.Cover.IndexOf(",");
            string coverWithoutMime = updateBookDto.Cover.Substring(startIndex + 1);
            Assert.Equal(coverWithoutMime, resultContent?.Data.Cover);

            // file are not retrieved as part of BookDto
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_WithInvalidInput_ReturnsErrorMessage()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.DeleteAsync("/api/Book/delete/0");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, book not found.", resultContent?.Message);
        }

        [Fact]
        public async Task Delete_WithValidInput_ReturnsBookWithAnonymousAddedByUserName()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);
            // Act
            var response = await _httpClient.DeleteAsync($"/api/Book/delete/{book.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (BookDto)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(book.Id, resultContent?.Data.Id);
            Assert.Equal("anonymous", resultContent?.Data.AddedByUserName);
        }
        #endregion

        #region AddToReadingList
        [Fact]
        public async Task AddToReadingList_WithInvalidInput_ReturnsErrorHandlingMiddlewareErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);

            BookToReadingListDto bookToReadingListDto = new BookToReadingListDto("someInexistentEmail@test.com", -1);

            string json = JsonConvert.SerializeObject(bookToReadingListDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/addToReadingList", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { status = false, message = (string)null! });

            Assert.False(resultContent?.status);
            Assert.Equal("An unexpected error occurred during processing.", resultContent?.message);
        }

        [Fact]
        public async Task AddToReadingList_WithValidInput_ReturnsSuccess()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);

            BookToReadingListDto bookToReadingListDto = new BookToReadingListDto(user.Email, book.Id);

            string json = JsonConvert.SerializeObject(bookToReadingListDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/addToReadingList", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false });

            Assert.True(resultContent?.Status);

            Assert.True(await TestHelper.IsBookInReadingListOfUser(book.Id, user.Id));
        }
        #endregion

        #region RemoveFromReadingList
        [Fact]
        public async Task RemoveFromReadingList_WithInvalidInput_ReturnsErrorHandlingMiddlewareErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            BookToReadingListDto bookToReadingListDto = new BookToReadingListDto("someInexistentEmail@test.com", -1);

            string json = JsonConvert.SerializeObject(bookToReadingListDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/addToReadingList", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { status = false, message = (string)null! });

            Assert.False(resultContent?.status);
            Assert.Equal("An unexpected error occurred during processing.", resultContent?.message);
        }

        [Fact]
        public async Task RemoveFromReadingList_WithValidInput_ReturnsSuccess()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            User userToAddBook = await TestHelper.AddNewUserToTestDatabase();
            Book bookToRemoveFromReadingList = await TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(userToAddBook.Id, user.Id);

            BookToReadingListDto bookToReadingListDto = new BookToReadingListDto(user.Email, bookToRemoveFromReadingList.Id);

            string json = JsonConvert.SerializeObject(bookToReadingListDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/removeFromReadingList", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false });

            Assert.True(resultContent?.Status);

            Assert.False(await TestHelper.IsBookInReadingListOfUser(bookToRemoveFromReadingList.Id, user.Id));

        }
        #endregion

        #region MarkAsReadOrUnread
        [Fact]
        public async Task MarkAsReadOrUnread_WithValidInput_ReturnsSuccess()
        {
            // Arrange, book is read 
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            User userToAddBook = await TestHelper.AddNewUserToTestDatabase();
            Book bookToMarkAsRead = await TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(userToAddBook.Id, user.Id);

            BookToReadingListDto bookToReadingListDto = new BookToReadingListDto(user.Email, bookToMarkAsRead.Id, true);

            string json = JsonConvert.SerializeObject(bookToReadingListDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/markAsReadOrUnread", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false });

            Assert.True(resultContent?.Status);

            Assert.True(await TestHelper.IsBookInReadingListOfUserRead(bookToMarkAsRead.Id, user.Id));

            // Arrange, book is not read 
            bookToReadingListDto = new BookToReadingListDto(user.Email, bookToMarkAsRead.Id, false);

            json = JsonConvert.SerializeObject(bookToReadingListDto);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            response = await _httpClient.PostAsync("/api/Book/markAsReadOrUnread", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            result = await response.Content.ReadAsStringAsync();

            resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false });

            Assert.True(resultContent?.Status);

            Assert.False(await TestHelper.IsBookInReadingListOfUserRead(bookToMarkAsRead.Id, user.Id));
        }
        #endregion

        #region GetStatistics
        [Fact]
        public async Task GetStatistics_WithValidInput_ReturnsBookStatisticsDto()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await TestHelper.AddNewBookToTestDatabase(user.Id);
            await TestHelper.AddNewBookToTestDatabase(user.Id);
            await TestHelper.AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(user.Id, user.Id);

            UserEmailDto userEmailDto = new UserEmailDto()
            {
                UserEmail = user.Email
            };

            var json = JsonConvert.SerializeObject(userEmailDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync($"/api/Book/getStatistics", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (BookStatisticsDto)null! });

            Assert.True(resultContent?.Status);
            Assert.True(resultContent?.Data.TotalBooks >= 3);
            Assert.True(resultContent?.Data.UploadedByUser == 3);
            Assert.True(resultContent?.Data.InUserReadingList == 1);
            Assert.True(resultContent?.Data.ReadByUser == 0);
        }
        #endregion

        #region GetBookForDownload
        [Fact]
        public async Task GetBookForDownload_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);

            BookToStringFileDto bookToStringFileDto = new BookToStringFileDto()
            {
                Id = -1
            };

            var json = JsonConvert.SerializeObject(bookToStringFileDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync($"/api/Book/getBookForDownload", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, book not found.", resultContent?.Message);
        }

        [Fact]
        public async Task GetBookForDownload_WithValidInput_ReturnsActionResult()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            string? token = TestHelper.GenerateToken(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Book book = await TestHelper.AddNewBookToTestDatabase(user.Id);

            BookToStringFileDto bookToStringFileDto = new BookToStringFileDto()
            {
                Id = book.Id
            };

            var json = JsonConvert.SerializeObject(bookToStringFileDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync($"/api/Book/getBookForDownload", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (string)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(Convert.ToBase64String(book.File), resultContent?.Data);
            Assert.Equal(FilesForTesting.Files.FileAsBase64WithoutMeme, resultContent?.Data);
        }
        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
            _customWebApplicationFactory?.Dispose();
        }
    }
}
