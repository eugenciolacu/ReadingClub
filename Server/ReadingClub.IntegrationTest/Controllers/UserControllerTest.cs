using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReadingClub.Infrastructure.DTO.User;
using System.Net;
using ReadingClub.Domain;
using System.Text;
using Moq;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.IntegrationTest.Controllers
{
    public class UserControllerTest : IDisposable
    {
        private CustomWebApplicationFactory _customWebApplicationFactory;
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserControllerTest()
        {
            _customWebApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _customWebApplicationFactory.CreateClient();

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            _connectionString = _configuration["ConnectionString"]!;
        }
        #region Create
        [Fact]
        public async Task Create_WithInvalidInput_ReturnsBadRequest()
        {
            // empty dto scenario
            // Arrange
            var createUserDtoWhenEmptyFields = new CreateUserDto() { };

            string jsonWhenEmptyFields = JsonConvert.SerializeObject(createUserDtoWhenEmptyFields);
            HttpContent contentWhenEmptyFields = new StringContent(jsonWhenEmptyFields, System.Text.Encoding.UTF8, "application/json");

            // Act
            var responseWhenEmptyFields = await _httpClient.PostAsync("/api/User/create", contentWhenEmptyFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenEmptyFields.StatusCode);

            var resultWhenEmptyFields = await responseWhenEmptyFields.Content.ReadAsStringAsync();

            var resultContentWhenEmptyFields = JsonConvert.DeserializeAnonymousType(resultWhenEmptyFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(4, resultContentWhenEmptyFields!.errors.Count);
            Assert.Equal("The User name field is required.", resultContentWhenEmptyFields!.errors["UserName"][0]);
            Assert.Equal("The Email address is required.", resultContentWhenEmptyFields!.errors["Email"][0]);
            Assert.Equal("Invalid Email address.", resultContentWhenEmptyFields!.errors["Email"][1]);
            Assert.Equal("The Password field is required.", resultContentWhenEmptyFields!.errors["Password"][0]);
            Assert.Equal("The Confirm password field is required.", resultContentWhenEmptyFields!.errors["ConfirmPassword"][0]);

            // wrong dto fields scenario 
            // Arrange
            var createUserDtoWhenInvalidFields = new CreateUserDto()
            {
                UserName = "Test",
                Email = "Test",
                Password = "Test",
                ConfirmPassword = "Something different",
            };

            string jsonWhenInvalidFields = JsonConvert.SerializeObject(createUserDtoWhenInvalidFields);
            HttpContent contentWhenInvalidFields = new StringContent(jsonWhenInvalidFields, System.Text.Encoding.UTF8, "application/json");

            // Act
            var responseWhenInvalidFields = await _httpClient.PostAsync("/api/User/create", contentWhenInvalidFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenInvalidFields.StatusCode);

            var resultWhenInvalidFields = await responseWhenInvalidFields.Content.ReadAsStringAsync();

            var resultContentWhenInvalidFields = JsonConvert.DeserializeAnonymousType(resultWhenInvalidFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(2, resultContentWhenInvalidFields!.errors.Count);
            Assert.Equal("Invalid Email address.", resultContentWhenInvalidFields!.errors["Email"][0]);
            Assert.Equal("The Password and Confirmation password do not match.", resultContentWhenInvalidFields!.errors["ConfirmPassword"][0]);
        }

        [Fact]
        public async Task Create_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            // Arrange userName exists
            var createUserDtoWithSameUserName = new CreateUserDto()
            {
                UserName = user.UserName,
                Email = "another" + user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password
            };

            string json1 = JsonConvert.SerializeObject(createUserDtoWithSameUserName);
            HttpContent content1 = new StringContent(json1, Encoding.UTF8, "application/json");

            // Act
            var response1 = await _httpClient.PostAsync("/api/User/create", content1);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            var result1 = await response1.Content.ReadAsStringAsync();

            var resultContent1 = JsonConvert.DeserializeAnonymousType(result1, new { Status = false, Message = (string)null! });

            Assert.False(resultContent1!.Status);
            Assert.Equal("This user name already exists.", resultContent1!.Message);

            // Arrange email exists
            var createUserDtoWithSameEmail = new CreateUserDto()
            {
                UserName = "another" + user.UserName,
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password
            };

            string json2 = JsonConvert.SerializeObject(createUserDtoWithSameEmail);
            HttpContent content2 = new StringContent(json2, Encoding.UTF8, "application/json");


            // Act
            var response2 = await _httpClient.PostAsync("/api/User/create", content2);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            var result2 = await response2.Content.ReadAsStringAsync();

            var resultContent2 = JsonConvert.DeserializeAnonymousType(result2, new { Status = false, Message = (string)null! });

            Assert.False(resultContent2!.Status);
            Assert.Equal("This email already exists.", resultContent2!.Message);

            // Arrange userName and email exists
            var createUserDtoWithSameUserNameAndEmail = new CreateUserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.Password
            };

            string json3 = JsonConvert.SerializeObject(createUserDtoWithSameUserNameAndEmail);
            HttpContent content3 = new StringContent(json3, Encoding.UTF8, "application/json");


            // Act
            var response3 = await _httpClient.PostAsync("/api/User/create", content3);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response3.StatusCode);

            var result3 = await response3.Content.ReadAsStringAsync();

            var resultContent3 = JsonConvert.DeserializeAnonymousType(result3, new { Status = false, Message = (string)null! });

            Assert.False(resultContent3!.Status);
            Assert.Equal("This user name and emai already exists.", resultContent3!.Message);
        }

        [Fact]
        public async Task Create_WithValidInput_ReturnsCreatedUserAsUserDto()
        {
            // Arrange
            string someGuidAsRandomValue = Guid.NewGuid().ToString();

            var createUserDto = new CreateUserDto()
            {
                UserName = someGuidAsRandomValue,
                Email = someGuidAsRandomValue + "@test.com",
                Password = "password",
                ConfirmPassword = "password"
            };

            string json = JsonConvert.SerializeObject(createUserDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (UserDto)null! });

            Assert.True(resultContent!.Status);
            Assert.Equal(createUserDto.UserName, resultContent.Data.UserName);
            Assert.Equal(createUserDto.Email, resultContent.Data.Email);
        }
        #endregion

        #region Login
        [Fact]
        public async Task Login_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var loginDtoWhenEmptyFields = new UserLoginDto() { };

            string jsonWhenEmptyFields = JsonConvert.SerializeObject(loginDtoWhenEmptyFields);
            HttpContent contentWhenEmptyFields = new StringContent(jsonWhenEmptyFields, Encoding.UTF8, "application/json");

            // Act
            var responseWhenEmptyFields = await _httpClient.PostAsync("/api/User/login", contentWhenEmptyFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenEmptyFields.StatusCode);

            var resultWhenEmptyFields = await responseWhenEmptyFields.Content.ReadAsStringAsync();

            var resultContentWhenEmptyFields = JsonConvert.DeserializeAnonymousType(resultWhenEmptyFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(2, resultContentWhenEmptyFields!.errors.Count);
            Assert.Equal("The Email field is required.", resultContentWhenEmptyFields!.errors["Email"][0]);
            Assert.Equal("The Password field is required.", resultContentWhenEmptyFields!.errors["Password"][0]);
        }

        [Fact]
        public async Task Login_WithValidInput_ReturnsTokenAsString()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            var loginDto = new UserLoginDto()
            {
                Email = user.Email,
                Password = TestHelper.OriginalPassword
            };

            string json = JsonConvert.SerializeObject(loginDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (string)null! });

            Assert.True(resultContent!.Status);
            Assert.NotNull(resultContent!.Data);
            Assert.NotEmpty(resultContent!.Data);
        }

        [Fact]
        public async Task Login_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            var loginDto = new UserLoginDto()
            {
                Email = "wrongEmail@test.com",
                Password = "wrongPassword"
            };

            string json = JsonConvert.SerializeObject(loginDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent!.Status);
            Assert.Equal("User do not exists or password is incorrect.", resultContent!.Message);
        }
        #endregion

        #region IsTokenValid
        [Fact]
        public async Task IsTokenValid_InvalidTokenResultingInNullUserDto_ReturnsErrorMessage()
        {
            // Arrange
            UserDto userDto = new UserDto()
            {
                UserName = "",
                Email = ""
            };

            string? token = TestHelper.GenerateToken(userDto);

            string json = JsonConvert.SerializeObject(new TokenDto() { Token = token });
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/isTokenValid", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent!.Status);
            Assert.Equal("Token validation failed, user not found.", resultContent!.Message);
        }

        [Fact]
        public async Task IsTokenValid_ValidToken_ReturnsTheSameToken()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);

            string json = JsonConvert.SerializeObject(new TokenDto() { Token = token });
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/isTokenValid", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (string)null! });

            Assert.True(resultContent!.Status);
            Assert.NotNull(resultContent!.Data);
            Assert.NotEmpty(resultContent!.Data);
            Assert.Equal(token, resultContent!.Data);
        }

        [Fact]
        public async Task IsTokenValid_TokenExpiredLesstTanAdayAgo_ReturnsNewToken()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? expiredToken = TestHelper.GenerateTokenThithExpirationOfLessThanADay(userDto);

            string json = JsonConvert.SerializeObject(new TokenDto() { Token = expiredToken });
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/isTokenValid", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (string)null! });

            Assert.True(resultContent!.Status);
            Assert.NotNull(resultContent!.Data);
            Assert.NotEmpty(resultContent!.Data);
            Assert.NotEqual(expiredToken, resultContent!.Data);
            
            string? validToken = TestHelper.GenerateToken(userDto);
            Assert.Equal(validToken, resultContent!.Data); 
        }

        [Fact]
        public async Task IsTokenValid_TokenExpiredMoreThanAdayAgo_ReturnsErrorMessage()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateTokenThithExpirationOfMoreThanADay(userDto);

            string json = JsonConvert.SerializeObject(new TokenDto() { Token = token });
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/isTokenValid", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent!.Status);
            Assert.Equal("Token validation failed.", resultContent!.Message);
        }

        [Fact]
        public async Task IsTokenValid_ThrowsErrorDuringTokenValidation_ReturnsErrorMessage()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateAlteredToken(userDto);

            string json = JsonConvert.SerializeObject(new TokenDto() { Token = token });
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/User/isTokenValid", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent!.Status);
            Assert.Equal("Token validation failed.", resultContent!.Message);
        }
        #endregion

        #region Get

        #endregion

        #region Update

        #endregion

        #region Delete

        #endregion

        #region GetLoggedUser

        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
            _customWebApplicationFactory?.Dispose();
        }
    }
}
