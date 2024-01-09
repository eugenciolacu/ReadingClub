using Newtonsoft.Json;
using ReadingClub.Infrastructure.DTO.User;
using System.Net;

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
        public async Task Create_WithValidInput_ReturnsCreatedUserAsUserDto()
        {
            // Arrange
            var createUserDto = new CreateUserDto()
            {
                UserName = "Test",
                Email = "test@test.com",
                Password = "password",
                ConfirmPassword = "password"
            };

            string json = JsonConvert.SerializeObject(createUserDto);
            HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

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

        public void Dispose()
        {
            _httpClient.Dispose();
            _customWebApplicationFactory?.Dispose();
        }
    }
}
