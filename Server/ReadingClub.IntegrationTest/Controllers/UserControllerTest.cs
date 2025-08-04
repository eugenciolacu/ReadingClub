using Newtonsoft.Json;
using ReadingClub.Infrastructure.DTO.User;
using System.Net;
using ReadingClub.Domain;
using System.Text;
using System.Net.Http.Headers;

namespace ReadingClub.IntegrationTest.Controllers
{
    public class UserControllerTest : IDisposable
    {
        private CustomWebApplicationFactory _customWebApplicationFactory;
        private HttpClient _httpClient;

        public UserControllerTest()
        {
            _customWebApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _customWebApplicationFactory.CreateClient();
        }

        #region Create
        [Fact]
        public async Task Create_WithInvalidInput_ReturnsBadRequest()
        {
            // empty dto scenario
            // Arrange
            var createUserDtoWhenEmptyFields = new CreateUserDto() { };

            string jsonWhenEmptyFields = JsonConvert.SerializeObject(createUserDtoWhenEmptyFields);
            HttpContent contentWhenEmptyFields = new StringContent(jsonWhenEmptyFields, Encoding.UTF8, "application/json");

            // Act
            var responseWhenEmptyFields = await _httpClient.PostAsync("/api/User/create", contentWhenEmptyFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenEmptyFields.StatusCode);

            var resultWhenEmptyFields = await responseWhenEmptyFields.Content.ReadAsStringAsync();

            var resultContentWhenEmptyFields = JsonConvert.DeserializeAnonymousType(resultWhenEmptyFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(4, resultContentWhenEmptyFields?.errors.Count);
            Assert.Equal("The User name field is required.", resultContentWhenEmptyFields?.errors["UserName"][0]);
            Assert.Equal("The Email address is required.", resultContentWhenEmptyFields?.errors["Email"][0]);
            Assert.Equal("Invalid Email address.", resultContentWhenEmptyFields?.errors["Email"][1]);
            Assert.Equal("The Password field is required.", resultContentWhenEmptyFields?.errors["Password"][0]);
            Assert.Equal("The Confirm password field is required.", resultContentWhenEmptyFields?.errors["ConfirmPassword"][0]);

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
            HttpContent contentWhenInvalidFields = new StringContent(jsonWhenInvalidFields, Encoding.UTF8, "application/json");

            // Act
            var responseWhenInvalidFields = await _httpClient.PostAsync("/api/User/create", contentWhenInvalidFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenInvalidFields.StatusCode);

            var resultWhenInvalidFields = await responseWhenInvalidFields.Content.ReadAsStringAsync();

            var resultContentWhenInvalidFields = JsonConvert.DeserializeAnonymousType(resultWhenInvalidFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(2, resultContentWhenInvalidFields?.errors.Count);
            Assert.Equal("Invalid Email address.", resultContentWhenInvalidFields?.errors["Email"][0]);
            Assert.Equal("The Password and Confirmation password do not match.", resultContentWhenInvalidFields?.errors["ConfirmPassword"][0]);
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

            Assert.False(resultContent1?.Status);
            Assert.Equal("This user name already exists.", resultContent1?.Message);

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

            Assert.False(resultContent2?.Status);
            Assert.Equal("This email already exists.", resultContent2?.Message);

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

            Assert.False(resultContent3?.Status);
            Assert.Equal("This user name and emai already exists.", resultContent3?.Message);
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

            Assert.True(resultContent?.Status);
            Assert.Equal(createUserDto.UserName, resultContent?.Data.UserName);
            Assert.Equal(createUserDto.Email, resultContent?.Data.Email);
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
            Assert.Equal(2, resultContentWhenEmptyFields?.errors.Count);
            Assert.Equal("The Email field is required.", resultContentWhenEmptyFields?.errors["Email"][0]);
            Assert.Equal("The Password field is required.", resultContentWhenEmptyFields?.errors["Password"][0]);
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

            Assert.True(resultContent?.Status);
            Assert.NotNull(resultContent?.Data);
            Assert.NotEmpty(resultContent?.Data);
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

            Assert.False(resultContent?.Status);
            Assert.Equal("User do not exists or password is incorrect.", resultContent?.Message);
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

            Assert.False(resultContent?.Status);
            Assert.Equal("Token validation failed, user not found.", resultContent?.Message);
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

            Assert.True(resultContent?.Status);
            Assert.NotNull(resultContent?.Data);
            Assert.NotEmpty(resultContent.Data);
            Assert.Equal(token, resultContent?.Data);
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

            Assert.True(resultContent?.Status);
            Assert.NotNull(resultContent?.Data);
            Assert.NotEmpty(resultContent.Data);
            Assert.NotEqual(expiredToken, resultContent?.Data);
            
            string? validToken = TestHelper.GenerateToken(userDto);
            Assert.Equal(validToken, resultContent?.Data); 
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

            Assert.False(resultContent?.Status);
            Assert.Equal("Token validation failed.", resultContent?.Message);
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

            Assert.False(resultContent?.Status);
            Assert.Equal("Token validation failed.", resultContent?.Message);
        }
        #endregion

        #region Get
        [Fact]
        public async Task Get_ClaimsIdentityNullEmail_ReturnsErrorResponse()
        {
            // Arrange
            UserDto userDto = new UserDto()
            {
                UserName = "",
                Email = ""
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.PostAsync("/api/User/getFullDetailsOfLoggedUser", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, user not found.", resultContent?.Message);
        }

        [Fact]
        public async Task Get_ValidClaimsIdentityInvalidEmail_ReturnsErrorResponse()
        {
            // Arrange
            UserDto userDto = new UserDto()
            {
                UserName = "someValidUserName",
                Email = "someValieEmail@test.com"
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.PostAsync("/api/User/getFullDetailsOfLoggedUser", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, user not found.", resultContent?.Message);
        }

        [Fact]
        public async Task Get_ValidClaimsIdentityValidEmail_ReturnsUserDto()
        {
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.PostAsync("/api/User/getFullDetailsOfLoggedUser", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (UserDto)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(userDto.UserName, resultContent?.Data.UserName);
            Assert.Equal(userDto.Email, resultContent?.Data.Email);
        }
        #endregion

        #region Update
        [Fact]
        public async Task Update_WithInvalidInput_ReturnsBadRequest()
        {
            // empty dto scenario
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateUserDtoWhenEmptyFields = new UpdateUserDto() { };

            string jsonWhenEmptyFields = JsonConvert.SerializeObject(updateUserDtoWhenEmptyFields);
            HttpContent contentWhenEmptyFields = new StringContent(jsonWhenEmptyFields, Encoding.UTF8, "application/json");

            // Act
            var responseWhenEmptyFields = await _httpClient.PutAsync("/api/User/update", contentWhenEmptyFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenEmptyFields.StatusCode);

            var resultWhenEmptyFields = await responseWhenEmptyFields.Content.ReadAsStringAsync();

            var resultContentWhenEmptyFields = JsonConvert.DeserializeAnonymousType(resultWhenEmptyFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(5, resultContentWhenEmptyFields!.errors.Count);
            Assert.Equal("The User name field is required.", resultContentWhenEmptyFields?.errors["UserName"][0]);
            Assert.Equal("The Email address is required.", resultContentWhenEmptyFields?.errors["Email"][0]);
            Assert.Equal("Invalid Email address.", resultContentWhenEmptyFields?.errors["Email"][1]);
            Assert.Equal("The Password field is required.", resultContentWhenEmptyFields?.errors["Password"][0]);
            Assert.Equal("The Confirm password field is required.", resultContentWhenEmptyFields?.errors["ConfirmPassword"][0]);
            Assert.Equal("The OldEmail field is required.", resultContentWhenEmptyFields?.errors["OldEmail"][0]);

            // wrong dto fields scenario 
            // Arrange
            var createUserDtoWhenInvalidFields = new UpdateUserDto()
            {
                UserName = "Test",
                Email = "Test",
                Password = "Test",
                ConfirmPassword = "Something different",
                OldEmail = user.Email,
                IsEditPassword = true,
            };

            string jsonWhenInvalidFields = JsonConvert.SerializeObject(createUserDtoWhenInvalidFields);
            HttpContent contentWhenInvalidFields = new StringContent(jsonWhenInvalidFields, Encoding.UTF8, "application/json");

            // Act
            var responseWhenInvalidFields = await _httpClient.PutAsync("/api/User/update", contentWhenInvalidFields);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseWhenInvalidFields.StatusCode);

            var resultWhenInvalidFields = await responseWhenInvalidFields.Content.ReadAsStringAsync();

            var resultContentWhenInvalidFields = JsonConvert.DeserializeAnonymousType(resultWhenInvalidFields,
                new { errors = (Dictionary<string, string[]>)null! });
            Assert.Equal(2, resultContentWhenInvalidFields?.errors.Count);
            Assert.Equal("Invalid Email address.", resultContentWhenInvalidFields?.errors["Email"][0]);
            Assert.Equal("The Password and Confirmation password do not match.", resultContentWhenInvalidFields?.errors["ConfirmPassword"][0]);
        }

        [Fact]
        public async Task Update_WithInvalidInput_ReturnsErrorResponse()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateUserDto = new UpdateUserDto() 
            {
                UserName = "NewUserName",
                Email = "newEmail@test.com",
                Password = "newPassword",
                ConfirmPassword = "newPassword",
                OldEmail = "wrongOldEmail@test.com",
                IsEditPassword = true
            };

            string json = JsonConvert.SerializeObject(updateUserDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/User/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, user not found.", resultContent?.Message);
        }

        [Fact]
        public async Task Update_WithValidInput_ReturnsUpdatedUserAsUserDto()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateUserDto = new UpdateUserDto()
            {
                UserName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString() + "@test.com",
                Password = "newPassword",
                ConfirmPassword = "newPassword",
                OldEmail = user.Email,
                IsEditPassword = true
            };

            string json = JsonConvert.SerializeObject(updateUserDto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PutAsync("/api/User/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { NewStatus = false, Data = (UserDto)null! });

            Assert.True(resultContent?.NewStatus);
            Assert.Equal(updateUserDto.UserName, resultContent?.Data.UserName);
            Assert.Equal(updateUserDto.Email.ToLower(), resultContent?.Data.Email);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_WithInvalidClaimsIdentity_ReturnsErrorResponse()
        {
            // Arrange
            UserDto userDto = new UserDto()
            {
                UserName = "",
                Email = ""
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.DeleteAsync("/api/User/deleteLoggedUser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, user not found.", resultContent?.Message);
        }

        [Fact]
        public async Task Delete_WithValidInput_ReturnsSuccess()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.DeleteAsync("/api/User/deleteLoggedUser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false });

            Assert.True(resultContent?.Status);
        }

        [Fact]
        public async Task Delete_UserServiceDeleteThrowsError_ReturnsErrorResponse()
        {
            // this situation cannot be tested.
            // It require to mock UserService or UserRepository to throw an Exception.
            // Mocking UserRepository.Delete cause to fail other tests.
            // If email is null or empty we do not reach delete code.
            // If email do not exisis in database nothing are deleted but no exception is thrown as SQL query is valid.
            // We could get exception here only if SQL code is broken or if we mock an exception
            // Left for later

            // Malicious input with potential SQL injection also did not worked to throw an error, users table is deleted
            // string maliciousUserEmail = "'maliciousEmail'; DROP TABLE users;";
            // string maliciousUserEmail = "'-- ' OR '1'='1'; DROP TABLE users; --";

            //// Arrange
            //User user = await TestHelper.AddNewUserToTestDatabase();

            //UserDto userDto = new UserDto()
            //{
            //    UserName = user.UserName,
            //    Email = user.Email,
            //};

            //string? token = TestHelper.GenerateToken(userDto);
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //// Act
            //var response = await _httpClient.DeleteAsync("/api/User/deleteLoggedUser");

            //// Assert
            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //var result = await response.Content.ReadAsStringAsync();

            //var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            //Assert.False(resultContent?.Status);
            //Assert.Equal("An error occurred during processing, cannot remove user.", resultContent?.Message);
        }

        #endregion

        #region GetLoggedUser
        [Fact]
        public async Task GetLoggedUser_WithValidToken_ReturnsLoggetUser()
        {
            // Arrange
            User user = await TestHelper.AddNewUserToTestDatabase();

            UserDto userDto = new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.PostAsync("/api/User/getLoggedUser", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Data = (UserDto)null! });

            Assert.True(resultContent?.Status);
            Assert.Equal(user.UserName, resultContent?.Data.UserName);
            Assert.Equal(user.Email.ToLower(), resultContent?.Data.Email);
        }

        [Fact]
        public async Task GetLoggedUser_WithInvalidToken_ReturnsErrorResponse()
        {
            // Arrange
            UserDto userDto = new UserDto()
            {
                UserName = "",
                Email = "",
            };

            string? token = TestHelper.GenerateToken(userDto);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _httpClient.PostAsync("/api/User/getLoggedUser", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var resultContent = JsonConvert.DeserializeAnonymousType(result, new { Status = false, Message = (string)null! });

            Assert.False(resultContent?.Status);
            Assert.Equal("An error occurred during processing, user not found.", resultContent?.Message);
        }
        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
            _customWebApplicationFactory?.Dispose();
        }
    }
}
