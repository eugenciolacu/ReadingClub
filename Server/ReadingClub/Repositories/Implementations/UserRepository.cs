using ReadingClub.Domain;
using ReadingClub.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ReadingClub.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(DatabaseConfig config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<User> Create(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = 
                    @$"INSERT INTO users (username, email, password, salt) 
                        VALUES(@UserName, @Email, @Password, @Salt); 
                    SELECT last_insert_rowid();";
                var queryResult = await connection.QueryAsync<int>(sqlQuery, user);
                int? userId = queryResult.FirstOrDefault();
                user.Id = (int)userId;
            }
            return await Get(user.Email);
        }

        public async Task<User> Update(User user, string oldEmail, bool isEditPassword)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = string.Empty;

                if (isEditPassword)
                {
                    sqlQuery = @"UPDATE users SET username = @UserName, email = @Email, 
                                password = @Password, salt = @Salt
                                WHERE email = @oldEmail";
                }
                else
                {
                    sqlQuery = @"UPDATE users SET username = @UserName, email = @Email
                                WHERE email = @oldEmail";
                }

                await connection.ExecuteAsync(sqlQuery, new
                {
                    user.UserName,
                    user.Email,
                    user.Password,
                    user.Salt,
                    oldEmail,
                });
            }
            return await Get(user.Email);
        }

        public async Task Delete(string userEmail)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = "DELETE FROM users WHERE email = @userEmail";
                await connection.ExecuteAsync(sqlQuery, new { userEmail });
            }
        }

        public async Task<User> Get(string email)
        {
            User u = null!;
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = 
                    $@"SELECT *
                    FROM users
                    WHERE email = @Email";
                var queryResult = await connection.QueryAsync<User>(sqlQuery, new { Email = email });

                var count = queryResult.Count();

                if (count == 1)
                {
                    u = queryResult.First();
                }
            }
            return u;
        }

        public async Task<int> CheckIfExists(string userName, string email)
        {
            // 0 - nothing, 1 - username exists, 2 - email exists, 3 - both exists
            var userNameSummary = 0;
            var emailSummary = 0;

            var sqlQuery =
                $@"SELECT COUNT(*)
                FROM users 
                WHERE userName = '{userName}';

                SELECT COUNT(*)
                FROM users 
                WHERE email = '{email}';";

            using (var connection = new SqliteConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery))
                {
                    var countByUserName = await multi.ReadAsync<int>();
                    var countByEmail = await multi.ReadAsync<int>();

                    userNameSummary = countByUserName.FirstOrDefault() >= 1 ? 1 : 0;
                    emailSummary = countByEmail.FirstOrDefault() >= 1 ? 2 : 0;
                }
            }

            return userNameSummary + emailSummary;
        }

        public async Task<int> GetUserIdByEmail(string email)
        {
            var id = 0;
            var count = 0;

            var sqlQuery =
                    $@"SELECT id
                    FROM users
                    WHERE email = '{email}';

                    SELECT COUNT(*)
                    FROM users
                    WHERE email = '{email}';";

            using (var connection = new SqliteConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery))
                {
                    var idByUserEmail = await multi.ReadAsync<int>();
                    var countByUserEmail = await multi.ReadAsync<int>();

                    id = idByUserEmail.FirstOrDefault();
                    count = countByUserEmail.FirstOrDefault();
                }
            }

            if (count == 1)
            {
                return id;
            }

            throw new Exception("User id not found, Email do not found.");
        }
    }
}
