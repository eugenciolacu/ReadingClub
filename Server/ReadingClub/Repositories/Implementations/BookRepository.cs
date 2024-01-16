using ReadingClub.Infrastructure.Common.Paging;
using ReadingClub.Domain;
using Dapper;
using Microsoft.Data.Sqlite;
using ReadingClub.Repositories.Interfaces;
using ReadingClub.Domain.Alternative;
using ReadingClub.Infrastructure.DTO.Book;
using System.Linq;

namespace ReadingClub.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(DatabaseConfig config)
        {
            _connectionString = config.ConnectionString;
        }

        public async Task<byte[]?> GetBookForDownload(int id)
        {
            byte[]? file = null!;

            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = "SELECT file FROM books WHERE id = @id";
                var queryResult = await connection.QueryAsync<byte[]>(sqlQuery, new { id });
                file = queryResult.FirstOrDefault();
            }

            return file;
        }

        public async Task<PagedResponse<BookExtra>> GetPagedAdminPage(PagedRequest pagedRequest)
        {
            int? id = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQueryAddedBy = @"SELECT id FROM users WHERE email = @UserEmail";
                var queryResult = await connection.QueryAsync<int>(sqlQueryAddedBy, pagedRequest);
                id = queryResult.FirstOrDefault()!;
            }

            List<BookExtra> books = new List<BookExtra>();
            var count = 0;

            pagedRequest.PageSize = pagedRequest.PageSize switch
            {
                5 => 5,
                10 => 10,
                20 => 20,
                50 => 50,
                _ => 5
            };

            // order by do not support params
            var orderBy = pagedRequest.OrderBy.ToLower() switch
            {
                "title" => "b.title",
                "authors" => "b.authors",
                "isbn" => "b.isbn",
                _ => "b.title"
            };

            var orderDirection = pagedRequest.OrderDirection == "Descending" ? "DESC" : "ASC";

            // ignore isbn when it is empty in request. Other fields are not null
            var isbnFilterSql = string.IsNullOrEmpty(pagedRequest.Filters["isbn"]) ? "" : " AND isbn LIKE @isbnFilterParam";

            var sqlQuery =
                @$"SELECT 
                    b.id, b.title, b.authors, b.isbn, b.description, b.cover, b.coverName, b.coverMime, b.fileName, b.addedBy,
                    (SELECT u.userName FROM users u WHERE u.id = b.addedBy) AS addedByUserName
                FROM 
                    books b
                WHERE
                    b.addedBy = {id}
                    AND b.title LIKE @titleFilterParam
                    AND b.authors LIKE @authorsFilterParam
                    {isbnFilterSql}
                ORDER BY {orderBy} {orderDirection}
                LIMIT @pageSizeParam 
                OFFSET (@pageParam - 1) * @pageSizeParam;

                SELECT COUNT(1) 
                FROM books
                WHERE
                    addedBy = {id}
                    AND title LIKE @titleFilterParam
                    AND authors LIKE @authorsFilterParam
                    {isbnFilterSql};";

            var parameters = new
            {
                titleFilterParam = "%" + pagedRequest.Filters["title"].Trim() + "%",
                authorsFilterParam = "%" + pagedRequest.Filters["authors"].Trim() + "%",
                isbnFilterParam = "%" + pagedRequest.Filters["isbn"].Trim() + "%",
                pageSizeParam = pagedRequest.PageSize,
                pageParam = pagedRequest.Page < 1 ? 1 : pagedRequest.Page
            };

            using (var connection = new SqliteConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, parameters))
                {
                    var b = await multi.ReadAsync<BookExtra>();
                    var c = await multi.ReadAsync<int>();

                    books = b.ToList();
                    count = c.FirstOrDefault();
                }
            }

            return new PagedResponse<BookExtra>(books, count);
        }

        public async Task<PagedResponse<BookExtra>> GetPagedSearchPage(PagedRequest pagedRequest)
        {
            List<BookExtra> books = new List<BookExtra>();
            var count = 0;

            pagedRequest.PageSize = pagedRequest.PageSize switch
            {
                5 => 5,
                10 => 10,
                20 => 20,
                50 => 50,
                _ => 5
            };

            // order by do not support params
            var orderBy = pagedRequest.OrderBy.ToLower() switch
            {
                "title" => "title",
                "authors" => "authors",
                "isbn" => "isbn",
                _ => "title"
            };

            var orderDirection = pagedRequest.OrderDirection == "Descending" ? "DESC" : "ASC";

            // ignore isbn when it is empty in request. Other fields are not null
            var isbnFilterSql = string.IsNullOrEmpty(pagedRequest.Filters["isbn"]) ? "" : " AND isbn LIKE @isbnFilterParam";

            var sqlQuery =
                $@"SELECT 
                    b.id, b.title, b.authors, b.isbn, b.description, b.cover, b.coverName, b.coverMime, b.fileName, b.addedBy, 
                    (SELECT u.userName FROM users u WHERE u.id = b.addedBy) as addedByUserName,
                    CASE
                        WHEN u.email IS NULL THEN 0
                        ELSE 1
                    END AS isInReadingList
                FROM 
                    books b 
                    LEFT JOIN usersBooks ub on ub.bookId = b.id
                    LEFT JOIN users u on u.id = ub.userId
                WHERE
                    title LIKE @titleFilterParam 
                    AND authors LIKE @authorsFilterParam
                    {isbnFilterSql}
                ORDER BY {orderBy} {orderDirection}
                LIMIT @pageSizeParam 
                OFFSET (@pageParam - 1) * @pageSizeParam;

                SELECT COUNT(1) 
                FROM books 
                WHERE
                    title LIKE @titleFilterParam
                    AND authors LIKE @authorsFilterParam
                    {isbnFilterSql};";

            var parameters = new
            {
                titleFilterParam = "%" + pagedRequest.Filters["title"].Trim() + "%",
                authorsFilterParam = "%" + pagedRequest.Filters["authors"].Trim() + "%",
                isbnFilterParam = "%" + pagedRequest.Filters["isbn"].Trim() + "%",
                pageSizeParam = pagedRequest.PageSize,
                pageParam = pagedRequest.Page < 1 ? 1 : pagedRequest.Page,
                userEmail = pagedRequest.UserEmail
            };
                
            using (var connection = new SqliteConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, parameters))
                {
                    var b = await multi.ReadAsync<BookExtra>();
                    var c = await multi.ReadAsync<int>();

                    books = b.ToList();
                    count = c.FirstOrDefault();
                }
            }

            return new PagedResponse<BookExtra>(books, count);
        }

        public async Task<PagedResponse<BookExtra>> GetPagedReadingListPage(PagedRequest pagedRequest)
        {
            List<BookExtra> books = new List<BookExtra>();
            var count = 0;

            pagedRequest.PageSize = pagedRequest.PageSize switch
            {
                5 => 5,
                10 => 10,
                20 => 20,
                50 => 50,
                _ => 5
            };

            // order by do not support params
            var orderBy = pagedRequest.OrderBy.ToLower() switch
            {
                "title" => "title",
                "authors" => "authors",
                "isbn" => "isbn",
                _ => "title"
            };
            var orderDirection = pagedRequest.OrderDirection == "Descending" ? "DESC" : "ASC";

            // ignore isbn when it is empty in request. Other fields are not null
            var isbnFilterSql = string.IsNullOrEmpty(pagedRequest.Filters["isbn"]) ? "" : " AND isbn LIKE @isbnFilterParam";

            var isRead = pagedRequest.Filters["isReadFilter"].ToLower() switch
            {
                "all" => "",
                "read" => " AND ub.isRead = 1",
                "not read" => " AND ub.isRead = 0",
                _ => ""
            };

            var sqlQuery =
                $@"SELECT 
                    b.id, b.title, b.authors, b.isbn, b.description, b.cover, b.coverName, b.coverMime, b.fileName, b.addedBy, 
                    (SELECT u.userName FROM users u WHERE u.id = b.addedBy) AS addedByUserName,
                    CASE
                        WHEN u.email IS NULL THEN 0
                        ELSE 1
                    END AS isInReadingList,
                    ub.isRead
                FROM 
                    books b 
                    LEFT JOIN usersBooks ub on ub.bookId = b.id
                    LEFT JOIN users u on u.id = ub.userId
                WHERE
                    u.email = @userEmail
                    AND isInReadingList = 1
                    {isRead}
                    AND b.title LIKE @titleFilterParam 
                    AND b.authors LIKE @authorsFilterParam
                    {isbnFilterSql}
                ORDER BY {orderBy} {orderDirection}
                LIMIT @pageSizeParam 
                OFFSET (@pageParam - 1) * @pageSizeParam;

                SELECT COUNT(1) FROM
                (SELECT 
	                CASE
		                WHEN u.email IS NULL THEN 0
		                ELSE 1
	                END AS isInReadingList
                FROM 
	                books b 
	                LEFT JOIN usersBooks ub on ub.bookId = b.id
	                LEFT JOIN users u on u.id = ub.userId
                WHERE
                    u.email = @userEmail
	                AND isInReadingList = 1
                    {isRead}
                    AND b.title LIKE @titleFilterParam
                    AND b.authors LIKE @authorsFilterParam
                    {isbnFilterSql});";

            var parameters = new
            {
                titleFilterParam = "%" + pagedRequest.Filters["title"].Trim() + "%",
                authorsFilterParam = "%" + pagedRequest.Filters["authors"].Trim() + "%",
                isbnFilterParam = "%" + pagedRequest.Filters["isbn"].Trim() + "%",
                pageSizeParam = pagedRequest.PageSize,
                pageParam = pagedRequest.Page < 1 ? 1 : pagedRequest.Page,
                userEmail = pagedRequest.UserEmail
            };

            using (var connection = new SqliteConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, parameters))
                {
                    var b = await multi.ReadAsync<BookExtra>();
                    var c = await multi.ReadAsync<int>();

                    books = b.ToList();
                    count = c.FirstOrDefault();
                }
            }

            return new PagedResponse<BookExtra>(books, count);
        }

        public async Task<Book> Get(int id)
        {
            Book book = null!;
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery =
                    @"SELECT b.*
                    FROM books b
                    INNER JOIN users u on u.id = b.addedBy 
                    WHERE b.id = @id;";
                var queryResult = await connection.QueryAsync<Book>(sqlQuery, new { id });
                book = queryResult.FirstOrDefault()!;
            }
            return book;
        }

        public async Task<BookExtra> GetExtra(int id)
        {
            BookExtra book = null!;
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery =
                    @"SELECT b.*, u.userName as AddedByUserName 
                    FROM books b
                    INNER JOIN users u on u.id = b.addedBy 
                    WHERE b.id = @id;";
                var queryResult = await connection.QueryAsync<BookExtra>(sqlQuery, new { id });
                book = queryResult.FirstOrDefault()!;
            }
            return book;
        }

        public async Task<Book> Create(Book book)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQueryInsert = 
                    $@"INSERT INTO books (title, authors, isbn, description, cover, coverName, coverMime, file, fileName, addedBy)
                        values(@Title, @Authors, @ISBN, @Description, @Cover, @CoverName, @CoverMime, @File, @FileName, @AddedBy);
                    SELECT last_insert_rowid();";
                var queryResult = await connection.QueryAsync<int>(sqlQueryInsert, book);
                int? bookId = queryResult.FirstOrDefault();
                book.Id = (int)bookId;
            }
            return await Get(book.Id);
        }

        public async Task<Book> Update(Book book, bool isCoverEdited, bool isFileEdited)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = "UPDATE books SET title = @Title, Authors = @Authors, ISBN = @ISBN, Description = @Description";

                if (isCoverEdited)
                {
                    sqlQuery += ", cover = @Cover, coverName = @CoverName, coverMime = @CoverMime";
                }

                if (isFileEdited)
                {
                    sqlQuery += ", file = @File, fileName = @FileName";
                }

                sqlQuery += " WHERE id = @Id;";

                await connection.ExecuteAsync(sqlQuery, book);
            }
            return await Get(book.Id);
        }

        public async Task<Book> DeleteUpdate(Book book)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery = "UPDATE books SET addedBy = @AddedBy WHERE id = @Id;";
                await connection.ExecuteAsync(sqlQuery, book);
            }
            return await Get(book.Id);
        }

        public async Task ClearBooksForUserBeforeDelete(string userEmail, int anonymousId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                // set uploaded books to anonymous
                // delete all from reading list
                var sqlQuery =
                    @"
                    BEGIN TRANSACTION;

                    UPDATE books SET addedBy = @anonymousId WHERE addedBy = (SELECT id from users WHERE email = @userEmail);

                    DELETE FROM usersBooks WHERE userId = (SELECT id FROM users WHERE email = @userEmail);

                    COMMIT;
                    ";

                try
                {
                    await connection.ExecuteAsync(sqlQuery, new { userEmail, anonymousId });
                }
                catch (Exception)
                {
                    await connection.ExecuteAsync("ROLLBACK;");
                    throw;
                }
            }
        }

        public async Task AddToReadingList(BookToReadingListDto bookToReadingListDto)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQueryInsert =
                    $@"INSERT INTO usersBooks (isRead, userId, bookId) 
                    VALUES (
                        @IsRead, 
                        (SELECT id FROM users WHERE email = @UserEmail), 
                        @BookId);";
                await connection.ExecuteAsync(sqlQueryInsert, bookToReadingListDto); 
            }
        }

        public async Task RemoveFromReadingList(BookToReadingListDto bookToReadingListDto)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQueryDelete =
                    $@"DELETE FROM usersBooks 
                        WHERE 
                            bookId = @BookId
                            and userId = (SELECT id FROM users WHERE email = @UserEmail);";
                await connection.ExecuteAsync(sqlQueryDelete, bookToReadingListDto);
            }
        }

        public async Task MarkAsReadOrUnread(BookToReadingListDto bookToReadingListDto)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                var sqlQuery =
                    $@"UPDATE usersBooks
                        SET isRead = @IsRead
                    WHERE 
                        bookId = @BookId
                        AND userId = (SELECT id FROM users WHERE email = @UserEmail);";
                await connection.ExecuteAsync(sqlQuery, bookToReadingListDto);
            }
        }

        public async Task<BookStatisticsDto> GetStatistics(string userEmail)
        {

            var sqlQuery = $@"
                SELECT COUNT(1) FROM books;
                
                SELECT COUNT(1) FROM books WHERE addedBy = (SELECT id FROM users WHERE email = @UserEmail);

                SELECT COUNT(1) FROM usersBooks WHERE userId = (SELECT id FROM users WHERE email = @UserEmail);

                SELECT COUNT(1) FROM usersBooks WHERE userId = (SELECT id FROM users WHERE email = @UserEmail) AND isRead = 1;
            ";

            var parameters = new
            {
                UserEmail = userEmail
            };

            BookStatisticsDto bookStatisticsDto = new BookStatisticsDto();

            using (var connection = new SqliteConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, parameters))
                {
                    var totalBooks = await multi.ReadAsync<int>();
                    var uploadedByUser = await multi.ReadAsync<int>();
                    var inUserReadingList = await multi.ReadAsync<int>();
                    var readByUser = await multi.ReadAsync<int>();

                    bookStatisticsDto.TotalBooks = totalBooks.FirstOrDefault();
                    bookStatisticsDto.UploadedByUser = uploadedByUser.FirstOrDefault();
                    bookStatisticsDto.InUserReadingList = inUserReadingList.FirstOrDefault();
                    bookStatisticsDto.ReadByUser = readByUser.FirstOrDefault();
                }
            }

            return bookStatisticsDto;
        }
    }
}
