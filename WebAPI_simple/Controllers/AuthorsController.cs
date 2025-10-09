using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Data;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        // Triển khai các Action tương ứng với IAuthorRepository
        [HttpGet("get-all-author")]
        public IActionResult GetAllAuthor()
        {
            var allAuthors = _authorRepository.GellAllAuthors();
            return Ok(allAuthors);
        }

        [HttpGet("get-author-by-id/{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var authorWithId = _authorRepository.GetAuthorById(id);
            if (authorWithId == null) return NotFound();
            return Ok(authorWithId);
        }

        [HttpPost("add-author")]
        [ValidateModel]
        public IActionResult AddAuthors([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok(authorAdd);
        }

        [HttpPut("update-author-by-id/{id}")]
        [ValidateModel]
        public IActionResult UpdateBookById(int id, [FromBody] AuthorNoIdDTO authorDTO)
        {
            var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
            if (authorUpdate == null) return NotFound();
            return Ok(authorUpdate);
        }

        [HttpDelete("delete-author-by-id/{id}")]
        public async Task<IActionResult> DeleteBookById(int id)
        {
            if (await _authorRepository.HasBooksAsync(id))
            {
                return BadRequest("Author still has associated books. Hãy gỡ liên kết trong Book_Author trước khi xóa.");
            }

            var authorDelete = _authorRepository.DeleteAuthorById(id);
            if (authorDelete == null) return NotFound();
            return Ok();
        }
    }
}