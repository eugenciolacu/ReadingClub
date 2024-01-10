using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Converters;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Helpers;

namespace ReadingClub.IntegrationTest
{
    public static class TestHelper
    {
        public static string OriginalPassword { get; } = "password";

        public static async Task<User> AddNewUserToTestDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            string someGuidAsRandomValue = Guid.NewGuid().ToString();
            var hashedPassword = PasswordHasher.HashPassword(OriginalPassword, "someSalt");

            User user = new User()
            {
                UserName = someGuidAsRandomValue,
                Email = someGuidAsRandomValue + "@test.com",
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
