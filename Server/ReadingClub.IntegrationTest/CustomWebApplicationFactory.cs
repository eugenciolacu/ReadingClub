using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ReadingClub.IntegrationTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = base.CreateWebHostBuilder();

            DeleteTestDatabase();

            return builder!;
        }

        private void DeleteTestDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var testDbPath = Path.GetFullPath(Directory.GetCurrentDirectory() + configuration["PartialPathToDB"]!);

            try
            {
                if(File.Exists(testDbPath))
                {
                    using var connection = new SqliteConnection(configuration["ConnectionString"]!);
                    connection.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(testDbPath);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}