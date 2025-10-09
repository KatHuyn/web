using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;

namespace WebAPI_simple.Controllers
{
    [Route("api/book-authors")]
    [ApiController]
    public class BookAuthorsController : ControllerBase
    {
        private readonly IBookAuthorRepository _bookAuthorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;

        public BookAuthorsController(IBookAuthorRepository bookAuthorRepository, IBookRepository bookRepository, IAuthorRepository authorRepository)
        {
            _bookAuthorRepository = bookAuthorRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
        }

        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> AddBookAuthor([FromBody] AddBookAuthorRequestDTO request)
        {
            bool bookExists = await _bookRepository.ExistsAsync(request.BookId);
            bool authorExists = await _authorRepository.ExistsAsync(request.AuthorId);

            if (!bookExists)
            {
                ModelState.AddModelError(nameof(request.BookId), $"Book ID {request.BookId} does not exist.");
            }

            if (!authorExists)
            {
                ModelState.AddModelError(nameof(request.AuthorId), $"Author ID {request.AuthorId} does not exist.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(ModelState);
            }

            if (await _bookAuthorRepository.ExistsRelationshipAsync(request.BookId, request.AuthorId))
            {
                return Conflict($"Relationship (BookID: {request.BookId}, AuthorID: {request.AuthorId}) already exists.");
            }

            var bookAuthor = _bookAuthorRepository.AddBookAuthor(request);
            return Ok(bookAuthor);
        }
    }
}