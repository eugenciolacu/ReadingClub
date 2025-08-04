using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Helpers;

namespace ReadingClub.SpecFlow.Support.Utilities
{
    public static class TestHelper
    {
        public static string OriginalPassword { get; } = "password";

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

        public static async Task<User> AddNewUserToTestDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            string someGuidAsRandomValue = Guid.NewGuid().ToString();
            var hashedPassword = PasswordHasher.HashPassword("password", "someSalt");

            User user = new User()
            {
                UserName = "user" + someGuidAsRandomValue,
                Email = "user" + someGuidAsRandomValue + "@test.com",
                Password = hashedPassword,
                Salt = "someSalt"
            };

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQuery =
                    @$"INSERT INTO users (username, email, password, salt) 
                        VALUES(@UserName, @Email, @Password, @Salt);
                    SELECT last_insert_rowid();";
                var queryResult = await connection.QueryAsync<int>(sqlQuery, user);
                int? userId = queryResult.FirstOrDefault();
                user.Id = (int)userId;
            }

            return user;
        }
    }
}
