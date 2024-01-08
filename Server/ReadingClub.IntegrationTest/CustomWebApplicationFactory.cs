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

            DeleteTestDatabase();

            builder.UseEnvironment("Test");
        }

        private void DeleteTestDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var testDbPath = Path.GetFullPath(Directory.GetCurrentDirectory() + configuration["PartialPathToDB"]!);

            FileInfo fi = new FileInfo(testDbPath);
            try
            {
                if(fi.Exists)
                {
                    using var connection = new SqliteConnection(configuration["ConnectionString"]!);
                    connection.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    fi.Delete();
                }
            }
            catch (Exception)
            {
                fi.Delete();
            }
        }
    }
}