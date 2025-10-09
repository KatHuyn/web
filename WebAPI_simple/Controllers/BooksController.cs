using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Data;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // <--- Đã thêm

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IAuthorRepository _authorRepository;

        private readonly ILogger<BooksController> _logger;

        public BooksController(AppDbContext dbContext, IBookRepository bookRepository,
                               IPublisherRepository publisherRepository, IAuthorRepository authorRepository,
                               ILogger<BooksController> logger)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
            _logger = logger;
        }

        [HttpGet("get-all-books")]
        [Authorize(Roles = "Read,Write")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllBooksQuery query)
        {

            _logger.LogInformation("GetAll Book Action method was invoked (Info)");
            _logger.LogWarning("This is a warning log (Warning)");
            _logger.LogError("This is a error log (Error)");

            var allBooks = await _bookRepository.GetAllBooks(query);

            if (allBooks == null || allBooks.Count == 0)
            {
                return NotFound("No books found matching the current filter, sort, or pagination criteria.");
            }

            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            if (bookWithIdDTO == null)
            {
                return NotFound();
            }
            return Ok(bookWithIdDTO);
        }
        [ValidateModel]
        [HttpPost("add-book")]
        [Authorize(Roles = "Write")]
        public async Task<ActionResult> AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {

            if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "Book must have at least one author.");
            }

            if (!await _publisherRepository.ExistsAsync(addBookRequestDTO.PublisherID))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID), $"Publisher ID {addBookRequestDTO.PublisherID} does not exist.");
            }

            if (await _bookRepository.IsTitleUniqueForPublisherAsync(addBookRequestDTO.Title, addBookRequestDTO.PublisherID))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Title), $"A book with title '{addBookRequestDTO.Title}' already exists for this publisher (ID: {addBookRequestDTO.PublisherID}).");
            }

            var year = addBookRequestDTO.DateAdded.Year;
            int publisherLimit = 100;
            if (await _publisherRepository.GetBooksCountForYearAsync(addBookRequestDTO.PublisherID, year) >= publisherLimit)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID), $"Publisher ID {addBookRequestDTO.PublisherID} has reached the limit of {publisherLimit} books in the year {year}.");
            }

            int authorLimit = 20;
            foreach (var authorId in addBookRequestDTO.AuthorIds.Distinct())
            {
                if (!await _authorRepository.ExistsAsync(authorId))
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"Author ID {authorId} does not exist.");
                }

                if (await _authorRepository.GetBooksCountAsync(authorId) >= authorLimit)
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"Author ID {authorId} has reached the limit of {authorLimit} published books.");
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(ModelState);
            }

            var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
            return Ok(bookAdd);
        }


        [HttpPut("update-book-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
            if (updateBook == null)
            {
                return NotFound();
            }
            return Ok(updateBook);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public ActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            if (deleteBook == null)
            {
                return NotFound();
            }
            return Ok(deleteBook);
        }
        #region Private methods 

        private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO), $"Please add book data");
                return false;
            }
            if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds),
                    $"Book must have at least one author.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}