using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private readonly User _testUser = null!;
        private readonly string? _testToken;

        private readonly PagedRequest _validPagedRequest;
        private readonly PagedRequest _invalidPagedRequest;

        public BookControllerTest()
        {
            _customWebApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _customWebApplicationFactory.CreateClient();

            _testUser = TestHelper.AddNewUserToTestDatabase().Result;
            UserDto userDto = new UserDto()
            {
                UserName = _testUser.UserName,
                Email = _testUser.Email,
            };
            _testToken = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            _validPagedRequest = new PagedRequest(5, 1, "title", "asc",
                new Dictionary<string, string>() {
                    { "title", "" },
                    { "authors", "" },
                    { "isbn", "" }
                },
                "someEmail");

            _invalidPagedRequest = new PagedRequest() { };
        }

        #region GetPagedAdminPage
        [Fact]
        public async Task GetPagedAdminPage_WithValidInput_ReturnsActionResult()
        {
            // Arrange
            string json = JsonConvert.SerializeObject(_validPagedRequest);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Book/getPagedAdminPage", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (PagedResponse<BookDto>)null! });

            Assert.True(resultContent?.Status);
            //Assert.Equal(updateUserDto.UserName, resultContent?.Data.UserName);
            //Assert.Equal(updateUserDto.Email.ToLower(), resultContent?.Data.Email);
        }
        #endregion

        #region GetPagedSearchPage

        #endregion

        #region GetPagedReadingListPage

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
