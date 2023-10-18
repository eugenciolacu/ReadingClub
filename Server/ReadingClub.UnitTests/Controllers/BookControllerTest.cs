using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using ReadingClub.Controllers;
using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Infrastructure.Middleware;
using ReadingClub.Services.Interfaces;
using System.Net;
using System.Net.Http;

namespace ReadingClub.UnitTests.Controllers
{
    public class BookControllerTest
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BookController _controller;

        private readonly PagedRequest _validPagedRequest;
        private readonly PagedRequest _invalidPagedRequest;

        public BookControllerTest()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BookController(_mockBookService.Object);

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
        public void GetPagedAdminPage_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetPagedAdminPage", typeof(HttpPostAttribute)));

        [Fact]
        public void GetPagedAdminPage_WithValidInput_ReturnsActionResult()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedAdminPage(It.IsAny<PagedRequest>()))
                .ReturnsAsync(new PagedResponse<BookDto>( new List<BookDto>(), 0 ));
            
            // Act
            var result = _controller.GetPagedAdminPage(_validPagedRequest);
            
            // Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("Status", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<BookDto>>(jsonAsString);
            Assert.IsType<PagedResponse<BookDto>>(pagedResponse);
        }

        [Fact]
        public void GetPagedAdminPage_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedAdminPage(It.IsAny<PagedRequest>()))
                .ThrowsAsync(new Exception("Simulate an error"));

            // Act
            var result = _controller.GetPagedAdminPage(_invalidPagedRequest);

            // Assert
            Assert.Equal("Simulate an error", result?.Exception?.InnerException?.Message);
        }

        [Fact]
        public async void GetPagedAdminPage_WithInvalidInput_ReturnsErrorResponse_AlternativeTestImplementation()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedAdminPage(It.IsAny<PagedRequest>()))
                .ThrowsAsync(new Exception("Simulate an error"));

            var json = JsonConvert.SerializeObject(_invalidPagedRequest);
            var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var testServer = new TestServer(
                new WebHostBuilder()
                    .Configure(app =>
                    {
                        app.UseMiddleware<ErrorHandlingMiddleware>();
                        app.Map("/callEndpoint", appBuilder =>
                        {
                            appBuilder.Run(
                                context => _controller.GetPagedAdminPage(_invalidPagedRequest)
                            );
                        });
                    })
            );

            var httpClient = testServer.CreateClient();

            // Act
            var response = await httpClient.PostAsync("/callEndpoint", httpContent);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.False((bool)errorResponse?.status);
            Assert.Equal("An unexpected error occurred during processing.", (string)errorResponse?.message! ?? null);
        }
        #endregion

        #region GetPagedSearchPage
        [Fact]
        public void GetPagedSearchPage_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetPagedSearchPage", typeof(HttpPostAttribute)));

        [Fact]
        public void GetPagedSearchPage_WithValidInput_ReturnsActionResult()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedSearchPage(It.IsAny<PagedRequest>()))
                .ReturnsAsync(new PagedResponse<BookDto>( new List<BookDto>(), 0 ));

            // Act
            var result = _controller.GetPagedSearchPage(_validPagedRequest);

            // Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("Status", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<BookDto>>(jsonAsString);
            Assert.IsType<PagedResponse<BookDto>>(pagedResponse);
        }

        [Fact]
        public void GetPagedSearchPage_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedSearchPage(It.IsAny<PagedRequest>()))
                .ThrowsAsync(new Exception("Simulate an error"));

            // Act
            var result = _controller.GetPagedSearchPage(_invalidPagedRequest);

            // Assert
            Assert.Equal("Simulate an error", result?.Exception?.InnerException?.Message);
        }
        #endregion

        #region GetPagedReadingListPage
        [Fact]
        public void GetPagedReadingListPage_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetPagedReadingListPage", typeof(HttpPostAttribute)));

        [Fact]
        public void GetPagedReadingListPage_WithValidInput_ReturnsActionResult()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedReadingListPage(It.IsAny<PagedRequest>()))
                .ReturnsAsync(new PagedResponse<BookDto>( new List<BookDto>(), 0 ));

            // Act
            var result = _controller.GetPagedReadingListPage( _validPagedRequest);

            // Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("Status", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<BookDto>>(jsonAsString);
            Assert.IsType<PagedResponse<BookDto>>(pagedResponse);
        }

        [Fact]
        public void GetPagedReadingListPage_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetPagedReadingListPage(It.IsAny<PagedRequest>()))
                .ThrowsAsync(new Exception("Simulate an error"));

            // Act
            var result = _controller.GetPagedReadingListPage(_invalidPagedRequest);

            // Assert
            Assert.Equal("Simulate an error", result?.Exception?.InnerException?.Message);
        }
        #endregion

        #region Create
        [Fact]
        public void Create_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "Create", typeof(HttpPostAttribute)));
        
        [Fact]
        public void Create_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var createBookDto = new CreateBookDto()
            {
                Title = null!,
                Authors = null!,
                ISBN = null,
                Description = null,
                Cover = null,
                CoverName = null,
                File = null!,
                FileName = null!,
                AddedByEmail = null!
            };

            // Act
            var result = _controller.Create(createBookDto);

            // Assert

        }

        [Fact]
        public void Create_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region Update
        [Fact]
        public void Update_ShouldHave_HttpPutAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "Update", typeof(HttpPutAttribute)));

        [Fact]
        public void Update_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region Delete
        [Fact]
        public void Delete_ShouldHave_HttpDeleteAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "Delete", typeof(HttpDeleteAttribute)));

        [Fact]
        public void Delete_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region AddToReadingList
        [Fact]
        public void AddToReadingList_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "AddToReadingList", typeof(HttpPostAttribute)));

        [Fact]
        public void AddToReadingList_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region RemoveFromReadingList
        [Fact]
        public void RemoveFromReadingList_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "RemoveFromReadingList", typeof(HttpPostAttribute)));

        [Fact]
        public void RemoveFromReadingList_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region MarkAsReadOrUnread
        [Fact]
        public void MarkAsReadOrUnread_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "MarkAsReadOrUnread", typeof(HttpPostAttribute)));

        [Fact]
        public void MarkAsReadOrUnread_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region GetStatistics
        [Fact]
        public void GetStatistics_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetStatistics", typeof(HttpPostAttribute)));

        [Fact]
        public void GetStatistics_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion

        #region GetBookForDownload
        [Fact]
        public void GetBookForDownload_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetBookForDownload", typeof(HttpPostAttribute)));

        [Fact]
        public void GetBookForDownload_WithValidInput_ReturnsActionResult()
        {

        }
        #endregion
    }
}
