using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Paging;
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

        #endregion

        #region Update

        #endregion

        #region Delete

        #endregion

        #region AddToReadingList

        #endregion

        #region RemoveFromReadingList

        #endregion

        #region MarkAsReadOrUnread

        #endregion

        #region GetStatistics

        #endregion

        #region GetBookForDownload

        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
            _customWebApplicationFactory?.Dispose();
        }
    }
}
