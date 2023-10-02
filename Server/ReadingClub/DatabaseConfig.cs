using Dapper;
using Microsoft.Data.Sqlite;
using ReadingClub.Infrastructure.Common.Helpers;

namespace ReadingClub
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; } = null!;

        public async void Setup()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var usersTable = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'users';");
                        var usersTableName = usersTable.FirstOrDefault();

                        if (!(!string.IsNullOrEmpty(usersTableName) && usersTableName == "users"))
                        {
                            var createUsersTableQuery =
                                @"CREATE TABLE users (
                                    id INTEGER PRIMARY KEY,
                                    userName TEXT(100) NOT NULL UNIQUE,
                                    email TEXT(320) NOT NULL UNIQUE,
                                    password TEXT NOT NULL,
                                    salt TEXT NOT NULL
                                );";
                            await connection.ExecuteAsync(createUsersTableQuery);

                            string salt = PasswordHasher.GenerateSalt();
                            string password = "clYqp~(tTO)ZBs^8U5Me;8~&lWF=yr,-(%=TwW85{-5[w]tL3.(oI4U~c;oj5A'Uhk46C,FdWu08sR=U1{tbO^)eL}K0v{a$L~j=";
                            string hashedPassword = PasswordHasher.HashPassword(password, salt);

                            var insertAdminQuery = 
                                @$"INSERT INTO users (userName, email, password, salt) 
                                VALUES (@userName, @email , @password, @salt);";
                            await connection.ExecuteAsync(insertAdminQuery, new { userName = "anonymous", email= "anonymous", password = hashedPassword, salt });
                        }

                        var booksTable = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'books';");
                        var bookTableName = booksTable.FirstOrDefault();

                        if (!(!string.IsNullOrEmpty(bookTableName) && bookTableName == "books"))
                        {
                            var createBooksTableQuery =
                                @"CREATE TABLE books (
                                    id INTEGER PRIMARY KEY,
                                    title TEXT NOT NULL,
                                    authors TEXT NOT NULL,
                                    isbn TEXT,
                                    description TEXT,
                                    cover BLOB,
                                    coverName TEXT,
                                    coverMime TEXT,
                                    file BLOB NOT NULL,
                                    fileName TEXT NOT NULL,
                                    addedBy INTEGER NOT NULL,
                            
                                    FOREIGN KEY(addedBy) REFERENCES users(id)
                                );";
                            await connection.QueryAsync(createBooksTableQuery);
                        }

                        var usersBooksTable = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'usersBooks';");
                        var usersBooksTableName = usersBooksTable.FirstOrDefault();

                        if (!(!string.IsNullOrEmpty(usersBooksTableName) && usersBooksTableName == "usersBooks"))
                        {
                            var createUsersBooksTableQuery =
                                @"CREATE TABLE usersBooks (
                                    id INTEGER PRIMARY KEY,
                                    isRead BOOLEAN NOT NULL CHECK (isRead in (0, 1)),
                                    userId INTEGER NOT NULL,
                                    bookId INTEGER NOT NULL,

                                    FOREIGN KEY(userId) REFERENCES users(id),
                                    FOREIGN KEY(bookId) REFERENCES books(id)
                                );";
                            await connection.QueryAsync(createUsersBooksTableQuery);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        
                        throw ex;
                    }
                }

                connection.Dispose();
            }
        }
    }
}
