using ReadingClub.Infrastructure.Common.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReadingClub.Infrastructure.DTO.Book;
using ReadingClub.Services.Interfaces;
using ReadingClub.Infrastructure.DTO;

namespace ReadingClub.Controllers
{
    [Authorize]
    [Route("api/[controller]/")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("getPagedAdminPage")]
        public async Task<ActionResult> GetPagedAdminPage(PagedRequest pagedRequest)
        {
            var pagedResponse = await _bookService.GetPagedAdminPage(pagedRequest);

            return Json(new { Status = true, Data = pagedResponse });
        }

        [HttpPost("getPagedSearchPage")]
        public async Task<ActionResult> GetPagedSearchPage(PagedRequest pagedRequest)
        {
            var pagedResponse = await _bookService.GetPagedSearchPage(pagedRequest);

            return Json(new { Status = true, Data = pagedResponse });
        }

        [HttpPost("getPagedReadingListPage")]
        public async Task<ActionResult> GetPagedReadingListPage(PagedRequest pagedRequest)
        {
            var pagedResponse = await _bookService.GetPagedReadingListPage(pagedRequest);

            return Json(new { Status = true, Data = pagedResponse });
        }

        //[HttpGet("details/{id}")]
        //public async Task<ActionResult> Get(int id)
        //{
        //    var bookDto = await _bookService.Get(id);

        //    if (bookDto == null)
        //    {
        //        return Json(new { Status = false, Message = "Book not found." });
        //    }

        //    return Json(new { Status = true, Data = bookDto });
        //}

        [HttpPost("create")]
        public async Task<ActionResult> Create(CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.IsValid);
            }

            var bookDto = await _bookService.Create(createBookDto);

            return Json(new { Status = true, Data = bookDto });
        }

        [HttpPut("update")]
        public async Task<ActionResult> Update(UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.IsValid);
            }

            var bookToUpdate = await _bookService.Get(updateBookDto.Id);

            if (bookToUpdate == null)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, book not found." });
            }

            var bookDto = await _bookService.Update(updateBookDto);

            return Json(new { Status = true, Data = bookDto });
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var bookToDelete = await _bookService.Delete(id);

            if (bookToDelete == null)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, book not found." });
            }

            return Json(new { Status = true, Data = bookToDelete });            
        }

        [HttpPost("addToReadingList")]
        public async Task<ActionResult> AddToReadingList(BookToReadingListDto bookToReadingListDto)
        {
            await _bookService.AddToReadingList(bookToReadingListDto);
            return Json(new { Status = true });
        }

        [HttpPost("removeFromReadingList")]
        public async Task<ActionResult> RemoveFromReadingList(BookToReadingListDto bookToReadingListDto)
        {
            await _bookService.RemoveFromReadingList(bookToReadingListDto);
            return Json(new { Status = true });
        }

        [HttpPost("markAsReadOrUnread")]
        public async Task<ActionResult> MarkAsReadOrUnread(BookToReadingListDto bookToReadingListDto)
        {
            await _bookService.MarkAsReadOrUnread(bookToReadingListDto);
            return Json(new { Status = true });
        }

        [HttpPost("getStatistics")]
        public async Task<ActionResult> GetStatistics(UserEmailDto userEmailDto)
        {
            var bookStatisticsDto = await _bookService.GetStatistics(userEmailDto.UserEmail);
            return Json(new { Status = true, Data = bookStatisticsDto });
        }

        [HttpPost("getBookForDownload")]
        public async Task<ActionResult> GetBookForDownload(BookToStringFileDto bookToStringFile)
        {
            var book = await _bookService.GetBookForDownload(bookToStringFile.Id);
            if (book == null)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, book not found." });
            }

            return Json(new { Status = true, Data = book });
        }
    }
}
