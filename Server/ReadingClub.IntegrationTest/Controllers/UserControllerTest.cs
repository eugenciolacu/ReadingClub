using Microsoft.AspNetCore.Mvc;
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
