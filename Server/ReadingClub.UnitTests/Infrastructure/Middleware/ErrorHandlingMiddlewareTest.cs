using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReadingClub.Infrastructure.Middleware;
using System.Net;

namespace ReadingClub.UnitTests.Infrastructure.Middleware
{
    public class ErrorHandlingMiddlewareTest
    {
        [Fact]
        public async Task MiddlewareHandlesException_ReturnsErrorResponse()
        {
            // Arrange
            var testServer = new TestServer(
                new WebHostBuilder()
                    .Configure(app =>
                    {
                        app.UseMiddleware<ErrorHandlingMiddleware>();
                        app.Map("/callEndpoint", appBuilder =>
                        {
                            appBuilder.Run(
                                context => throw new Exception("Test exception")
                            );
                        });
                    })
            );

            var httpClient = testServer.CreateClient();

            // Act
            var response = await httpClient.GetAsync("/callEndpoint");

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.False((bool)errorResponse?.status);
            Assert.Equal("An unexpected error occurred during processing.", (string)errorResponse?.message! ?? null);
        }
    }
}
