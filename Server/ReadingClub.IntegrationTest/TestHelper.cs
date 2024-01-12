using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReadingClub.Domain;
using ReadingClub.Infrastructure.Common.Helpers;
using ReadingClub.Infrastructure.DTO.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReadingClub.IntegrationTest
{
    public static class TestHelper
    {
        public static string OriginalPassword { get; } = "password";

        //public static async Task<Book> AddNewBookToTestDatabase(int addedByUserId)
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.Test.json")
        //        .Build();

        //    string someGuidAsRandomValue = Guid.NewGuid().ToString();

        //    Book book = new Book()
        //    {
        //        Title = "Title" + someGuidAsRandomValue,
        //        Authors = "Authors" + someGuidAsRandomValue,
        //        ISBN = "ISBN" + someGuidAsRandomValue,
        //        Description = "Description" + someGuidAsRandomValue,

        //    };
        //}

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

        public static string GenerateToken(UserDto userDto)
        {
            return GenerateToken(userDto, DateTime.Now.AddMinutes(1));
        }

        public static string GenerateTokenThithExpirationOfLessThanADay(UserDto userDto)
        {
            return GenerateToken(userDto, DateTime.Now.AddMinutes(-10));
        }

        public static string GenerateTokenThithExpirationOfMoreThanADay(UserDto userDto)
        {
            return GenerateToken(userDto, DateTime.Now.AddDays(-1.1));
        }

        public static string GenerateAlteredToken(UserDto userDto)
        {
            var token = GenerateToken(userDto);
            var alteredToken = token.Substring(0, token.Length - 1);
            return alteredToken;
        }

        private static string GenerateToken(UserDto userDto, DateTime expires)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userDto.UserName),
                new Claim(ClaimTypes.Email, userDto.Email)
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
