using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ReadingClub.SpecFlow.Support.Utilities
{
    public static class TestHelper
    {
        public static async Task DeleteTables()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQueryDropuUersBooks = "DROP TABLE usersBooks;";
                await connection.ExecuteAsync(sqlQueryDropuUersBooks);

                var sqlQueryDropBooks = "DROP TABLE books;";
                await connection.ExecuteAsync(sqlQueryDropBooks);

                var sqlQueryDropUsers = "DROP TABLE users;";
                await connection.ExecuteAsync(sqlQueryDropUsers);
            }
        }
    }
}
