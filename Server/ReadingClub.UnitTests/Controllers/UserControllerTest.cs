using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadingClub.Controllers;
using ReadingClub.Infrastructure.DTO.User;
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
            var json = @"
            {
                ""Jwt"": {
                    ""Key"": ""ReadingClub secret key"",
                    ""Issuer"": ""http://localhost:62360/"",
                    ""Audience"": ""http://localhost:62360/""
                }
            }";
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns(JObject.Parse(json)["Jwt"]["Key"].ToString());
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns(JObject.Parse(json)["Jwt"]["Issuer"].ToString());
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns(JObject.Parse(json)["Jwt"]["Audience"].ToString());

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


        [Fact]
        public void Create_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var createUserDtoWhenEmptyFields = new CreateUserDto() { };

            _controller.ModelState.AddModelError("UserName", "The User name field is required.");
            _controller.ModelState.AddModelError("Email", "The Email address is required.");
            _controller.ModelState.AddModelError("ConfirmPassword", "The Confirm cassword field is required.");

            var createUserDtoWhenInvalidFields = new CreateUserDto() 
            { 
                UserName = "Test",
                Email = "Test",
                Password = "Test",
                ConfirmPassword = "Something different",
            };

            _controller.ModelState.AddModelError("Email", "Invalid Email address.");
            _controller.ModelState.AddModelError("ConfirmPassword", "The Password and Confirmation password do not match.");

            // Act
            var resultWhenEmptyFields = _controller.Create(createUserDtoWhenEmptyFields);

            var resultWhenInvalidFields = _controller.Create(createUserDtoWhenInvalidFields);

            // Assert
            Assert.IsType<Task<ActionResult>>(resultWhenEmptyFields);
            Assert.Equal(400, (resultWhenEmptyFields.Result as ObjectResult)?.StatusCode);
            var jsonAsString1 = JsonConvert.SerializeObject(resultWhenEmptyFields.Result);
            Assert.Contains("Parameter is missing", jsonAsString1);

            Assert.IsType<Task<ActionResult>>(resultWhenInvalidFields);
            Assert.Equal(400, (resultWhenInvalidFields.Result as ObjectResult)?.StatusCode);
            var jsonAsString2 = JsonConvert.SerializeObject(resultWhenEmptyFields.Result);
            Assert.Contains("Parameter is missing", jsonAsString2);
        }

        [Fact]
        public void Create_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            _mockUserService.SetupSequence(service => service.CheckIfUserExists(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(1)
                .ReturnsAsync(2)
                .ReturnsAsync(3);

            // Act
            var result1 = _controller.Create(new CreateUserDto() { });
            var result2 = _controller.Create(new CreateUserDto() { });
            var result3 = _controller.Create(new CreateUserDto() { });

            // Assert
            var jsonAsString1 = JsonConvert.SerializeObject(result1.Result);
            Assert.Contains("\"Status\":false", jsonAsString1);
            Assert.Contains("\"Message\":\"This user name already exists.\"", jsonAsString1);

            var jsonAsString2 = JsonConvert.SerializeObject(result2.Result);
            Assert.Contains("\"Status\":false", jsonAsString2);
            Assert.Contains("\"Message\":\"This email already exists.\"", jsonAsString2);

            var jsonAsString3 = JsonConvert.SerializeObject(result3.Result);
            Assert.Contains("\"Status\":false", jsonAsString3);
            Assert.Contains("\"Message\":\"This user name and emai already exists.\"", jsonAsString3);
        }

        [Fact]
        public void Create_WithValidInput_ReturnsActionResult()
        {
            // Arrange
            _mockUserService.Setup(service => service.Create(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(new UserDto() { });

            // Act
            var result = _controller.Create(new CreateUserDto() { });

            // Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("\"Status\":true", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var userDto = JsonConvert.DeserializeObject<UserDto>(jsonAsString);
            Assert.IsType<UserDto>(userDto);
            Assert.NotNull(userDto);
        }

        #endregion

        #region Login
        [Fact]
        public void Login_ShouldHave_AllowAnonymous() =>
           Assert.True(TestHelper.IsAttributePresent(_controller, "Login", typeof(AllowAnonymousAttribute)));

        [Fact]
        public void Login_ShouldHave_HttpPostAction() =>
            Assert.True(TestHelper.IsAttributePresent(_controller, "Login", typeof(HttpPostAttribute)));

        [Fact]
        public void Login_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "The Email field is required.");
            _controller.ModelState.AddModelError("Password", "The Password field is required.");

            // Act
            var result = _controller.Login(new UserLoginDto() { });

            // Asserst
            Assert.IsType<Task<ActionResult>>(result);
            Assert.Equal(400, (result.Result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public void Login_WithValidInput_ReturnsActionResult()
        {
            var userDto = new UserDto()
            {
                UserName = "someValidUser",
                Email = "someValidEmail@gmail.com"
            };

            // Arrange
            _mockUserService.Setup(service => service.Get(It.IsAny<UserLoginDto>()))
                .ReturnsAsync(userDto);

            // Act
            var result = _controller.Login(new UserLoginDto() {
                Email = "someValidEmail@gmail.com",
                Password = "someValidPAssword"
            });

            // Assert
            Assert.IsType<Task<ActionResult>>(result);

            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("\"Status\":true", jsonAsString);
            Assert.Contains("Data", jsonAsString);

            var token = JsonConvert.DeserializeObject<dynamic>(jsonAsString)!["Value"]["Data"];
            Assert.NotNull(token);
        }

        [Fact]
        public void Login_WithInvalidInput_ReturnsErrorResponse()
        {
            UserDto? userDto = null;

            _mockUserService.Setup(service => service.Get(It.IsAny<UserLoginDto>()))
                !.ReturnsAsync(userDto);

            // Act
            var result = _controller.Login(new UserLoginDto() { });

            // Assert
            var jsonAsString = JsonConvert.SerializeObject(result.Result);
            Assert.Contains("\"Status\":false", jsonAsString);
            Assert.Contains("\"Message\":\"User do not exists or password is incorrect.\"", jsonAsString);
        }

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
