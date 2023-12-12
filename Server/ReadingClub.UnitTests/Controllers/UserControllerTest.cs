using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ReadingClub.Controllers;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.UnitTests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _mockUserService= new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new UserController(_mockUserService.Object, _mockConfiguration.Object);
        }

        #region class level tests
        [Fact]
        public void UserController_ShouldHave_RouteAttribute() =>
            Assert.True(TestHelper.IsAttributePresentAtControllerLevel(_controller, typeof(RouteAttribute)));

        [Fact]
        public void UserController_ShouldHave_ApiControllerAttribute() =>
            Assert.True(TestHelper.IsAttributePresentAtControllerLevel(_controller, typeof(ApiControllerAttribute)));

        #endregion

        #region Create
        [Fact]
        public void Create_ShouldHave_AllowAnonymous() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "Create", typeof(AllowAnonymousAttribute)));

        [Fact]
        public void Create_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "Create", typeof(HttpPostAttribute)));
        #endregion

        #region Login
        [Fact]
        public void Login_ShouldHave_AllowAnonymous() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "Login", typeof(AllowAnonymousAttribute)));

        [Fact]
        public void Login_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "Login", typeof(HttpPostAttribute)));
        #endregion

        #region IsTokenValid
        [Fact]
        public void IsTokenValid_ShouldHave_Authorize() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "IsTokenValid", typeof(AllowAnonymousAttribute)));

        [Fact]
        public void IsTokenValid_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "IsTokenValid", typeof(HttpPostAttribute)));
        #endregion

        #region Get
        [Fact]
        public void Get_ShouldHave_Authorize() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "Get", typeof(AuthorizeAttribute)));

        [Fact]
        public void Get_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "Get", typeof(HttpPostAttribute)));
        #endregion

        #region Update
        [Fact]
        public void Update_ShouldHave_Authorize() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "Update", typeof(AuthorizeAttribute)));

        [Fact]
        public void Update_ShouldHave_HttpPutAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "Update", typeof(HttpPutAttribute)));
        #endregion

        #region Delete
        [Fact]
        public void Delete_ShouldHave_Authorize() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "Delete", typeof(AuthorizeAttribute)));

        [Fact]
        public void Delete_ShouldHave_HttpDeleteAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "Delete", typeof(HttpDeleteAttribute)));
        #endregion

        #region GetLoggedUser
        [Fact]
        public void GetLoggedUser_ShouldHave_Authorize() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "GetLoggedUser", typeof(AuthorizeAttribute)));

        [Fact]
        public void GetLoggedUser_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "GetLoggedUser", typeof(HttpPostAttribute)));
        #endregion
    }
}
