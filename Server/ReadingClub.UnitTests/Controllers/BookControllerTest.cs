using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using ReadingClub.Controllers;
using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.UnitTests.Controllers
{
    public class BookControllerTest
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BookController _controller;

        private readonly PagedRequest _validPagedRequest;

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
        }

        #region GetPagedAdminPage
        [Fact]
        public void GetPagedAdminPage_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetPagedAdminPage", typeof(HttpPostAttribute)));

        [Fact]
        public void GetPagedAdminPage_WithValidInput_ReturnsActionResult()
        {
            #region Arrange
            _mockBookService.Setup(service => service.GetPagedAdminPage(_validPagedRequest))
                .ReturnsAsync(new PagedResponse<BookDto>( new List<BookDto>(), 0 ));
            #endregion

            #region Act
            var result = _controller.GetPagedAdminPage(_validPagedRequest);
            #endregion

            #region Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("Status", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<BookDto>>(jsonAsString);
            Assert.IsType<PagedResponse<BookDto>>(pagedResponse);
            #endregion
        }
        #endregion

        #region GetPagedSearchPage
        [Fact]
        public void GetPagedSearchPage_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetPagedSearchPage", typeof(HttpPostAttribute)));

        [Fact]
        public void GetPagedSearchPage_WithValidInput_ReturnsActionResult()
        {
            #region Arrange
            _mockBookService.Setup(service => service.GetPagedSearchPage(_validPagedRequest))
                .ReturnsAsync(new PagedResponse<BookDto>( new List<BookDto>(), 0 ));
            #endregion

            #region Act
            var result = _controller.GetPagedSearchPage(_validPagedRequest);
            #endregion

            #region Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("Status", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<BookDto>>(jsonAsString);
            Assert.IsType<PagedResponse<BookDto>>(pagedResponse);
            #endregion
        }
        #endregion

        #region GetPagedReadingListPage
        [Fact]
        public void GetPagedReadingListPage_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "GetPagedReadingListPage", typeof(HttpPostAttribute)));

        [Fact]
        public void GetPagedReadingListPage_WithValidInput_ReturnsActionResult()
        {
            #region Arrange
            _mockBookService.Setup(service => service.GetPagedReadingListPage(_validPagedRequest))
                .ReturnsAsync(new PagedResponse<BookDto>( new List<BookDto>(), 0 ));
            #endregion

            #region Act
            var result = _controller.GetPagedReadingListPage( _validPagedRequest);
            #endregion

            #region Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("Status", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<BookDto>>(jsonAsString);
            Assert.IsType<PagedResponse<BookDto>>(pagedResponse);
            #endregion
        }
        #endregion

        #region Create
        [Fact]
        public void Create_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsHttpActionAttributePresent(_controller, "Create", typeof(HttpPostAttribute)));
        
        [Fact]
        public void Create_WithInvalidInput_ReturnsBadRequest()
        {
            #region Arrange
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
            #endregion

            #region Act
            var result = _controller.Create(createBookDto);
            #endregion

            #region Assert
            #endregion
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
