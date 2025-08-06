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

        public static async Task<bool> IsBookInReadingListOfUserRead(int bookId, int userId)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQuery =
                    @"SELECT isRead FROM usersBooks WHERE userId = @userId AND bookId = @bookId";
                var queryResult = await connection.QueryAsync<int>(sqlQuery, new { userId, bookId });

                return queryResult.FirstOrDefault() == 1;
            }
        }

        public static async Task<bool> IsBookInReadingListOfUser(int bookId, int userId)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQuery =
                    @"SELECT COUNT(1) FROM usersBooks WHERE userId = @userId AND bookId = @bookId";
                var queryResult = await connection.QueryAsync<int>(sqlQuery, new { userId, bookId });

                return queryResult.FirstOrDefault() == 1;
            }
        }

        public static async Task<Book> AddNewBookToTestDatabaseThenToReadingListOfSpecificUser(int addedByUserId, int specificUserId)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            string someGuidAsRandomValue = Guid.NewGuid().ToString();

            Book book = new Book()
            {
                Title = "Title" + someGuidAsRandomValue,
                Authors = "Authors" + someGuidAsRandomValue,
                ISBN = "ISBN" + someGuidAsRandomValue,
                Description = "Description" + someGuidAsRandomValue,
                Cover = Convert.FromBase64String(FilesForTesting.Files.CoverAsBase64WithoutMeme),
                CoverName = "CoverName" + someGuidAsRandomValue,
                CoverMime = FilesForTesting.Files.CoverAsBase64Meme,
                File = Convert.FromBase64String(FilesForTesting.Files.FileAsBase64WithoutMeme),
                FileName = "FileName" + someGuidAsRandomValue,
                AddedBy = addedByUserId
            };

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQueryInsertBook =
                    $@"INSERT INTO books (title, authors, isbn, description, cover, coverName, coverMime, file, fileName, addedBy)
                        values(@Title, @Authors, @ISBN, @Description, @Cover, @CoverName, @CoverMime, @File, @FileName, @AddedBy);
                    SELECT last_insert_rowid();";
                var queryResult = await connection.QueryAsync<int>(sqlQueryInsertBook, book);
                int? bookId = queryResult.FirstOrDefault();
                book.Id = (int)bookId;

                var sqlQueryInsertInUsersBooks =
                    $@"INSERT INTO usersBooks (isRead, userId, bookId) 
                    VALUES (
                        0, 
                        {specificUserId}, 
                        {book.Id});";
                await connection.ExecuteAsync(sqlQueryInsertInUsersBooks);
            }

            return book;
        }

        public static async Task<Book> AddNewBookOfSpecificAuthorToTestDatabase(int addedByUserId, string authors)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            string someGuidAsRandomValue = Guid.NewGuid().ToString();

            Book book = new Book()
            {
                Title = "Title" + someGuidAsRandomValue,
                Authors = "Authors" + authors,
                ISBN = "ISBN" + someGuidAsRandomValue,
                Description = "Description" + someGuidAsRandomValue,
                Cover = Convert.FromBase64String(FilesForTesting.Files.CoverAsBase64WithoutMeme),
                CoverName = "CoverName" + someGuidAsRandomValue,
                CoverMime = FilesForTesting.Files.CoverAsBase64Meme,
                File = Convert.FromBase64String(FilesForTesting.Files.FileAsBase64WithoutMeme),
                FileName = "FileName" + someGuidAsRandomValue,
                AddedBy = addedByUserId
            };

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQueryInsert =
                    $@"INSERT INTO books (title, authors, isbn, description, cover, coverName, coverMime, file, fileName, addedBy)
                        values(@Title, @Authors, @ISBN, @Description, @Cover, @CoverName, @CoverMime, @File, @FileName, @AddedBy);
                    SELECT last_insert_rowid();";
                var queryResult = await connection.QueryAsync<int>(sqlQueryInsert, book);
                int? bookId = queryResult.FirstOrDefault();
                book.Id = (int)bookId;
            }

            return book;
        }

        public static async Task<Book> AddNewBookToTestDatabase(int addedByUserId)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var connectionString = configuration["ConnectionString"]!;

            string someGuidAsRandomValue = Guid.NewGuid().ToString();

            Book book = new Book()
            {
                Title = "Title" + someGuidAsRandomValue,
                Authors = "Authors" + someGuidAsRandomValue,
                ISBN = "ISBN" + someGuidAsRandomValue,
                Description = "Description" + someGuidAsRandomValue,
                Cover = Convert.FromBase64String(FilesForTesting.Files.CoverAsBase64WithoutMeme),
                CoverName = "CoverName" + someGuidAsRandomValue,
                CoverMime = FilesForTesting.Files.CoverAsBase64Meme,
                File = Convert.FromBase64String(FilesForTesting.Files.FileAsBase64WithoutMeme),
                FileName = "FileName" + someGuidAsRandomValue,
                AddedBy = addedByUserId
            };

            using (var connection = new SqliteConnection(connectionString))
            {
                var sqlQueryInsert =
                    $@"INSERT INTO books (title, authors, isbn, description, cover, coverName, coverMime, file, fileName, addedBy)
                        values(@Title, @Authors, @ISBN, @Description, @Cover, @CoverName, @CoverMime, @File, @FileName, @AddedBy);
                    SELECT last_insert_rowid();";
                var queryResult = await connection.QueryAsync<int>(sqlQueryInsert, book);
                int? bookId = queryResult.FirstOrDefault();
                book.Id = (int)bookId;
            }

            return book;
        }

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
