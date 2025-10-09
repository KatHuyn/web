using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Data;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_simple.Controllers
{
    // Đặt tên Controller là AuthorsController
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;

        // Dependency Injection
        public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        // GET: /api/Authors/get-all-author
        // [Authorize(Roles = "Read")] // Nếu muốn bảo vệ endpoint
        [HttpGet("get-all-author")]
        public IActionResult GetAllAuthor()
        {
            var allAuthors = _authorRepository.GellAllAuthors();
            return Ok(allAuthors);
        }

        // GET: /api/Authors/get-author-by-id/{id}
        [HttpGet("get-author-by-id/{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var authorWithId = _authorRepository.GetAuthorById(id);
            if (authorWithId == null) return NotFound();
            return Ok(authorWithId);
        }

        // POST: /api/Authors/add-author
        [HttpPost("add-author")]
        [ValidateModel]
        // [Authorize(Roles = "Write")] // Nếu muốn bảo vệ endpoint
        public async Task<ActionResult> AddAuthor([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (await _authorRepository.ExistsAsync(0)) // Giả sử dùng ExistsAsync để kiểm tra tên duy nhất nếu cần
            {
                // Thêm kiểm tra validation ở đây nếu cần, nhưng logic cơ bản là thêm tác giả
            }

            var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok(authorAdd);
        }

        // PUT: /api/Authors/update-author-by-id/{id}
        [HttpPut("update-author-by-id/{id}")]
        [ValidateModel]
        // [Authorize(Roles = "Write")] // Nếu muốn bảo vệ endpoint
        public IActionResult UpdateAuthorById(int id, [FromBody] AuthorNoIdDTO authorDTO)
        {
            var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
            if (authorUpdate == null) return NotFound();
            return Ok(authorUpdate);
        }

        // DELETE: /api/Authors/delete-author-by-id/{id}
        [HttpDelete("delete-author-by-id/{id}")]
        // [Authorize(Roles = "Write")] // Nếu muốn bảo vệ endpoint
        public async Task<ActionResult> DeleteAuthorById(int id)
        {
            // Kiểm tra xem Author có liên kết với sách nào không
            if (await _authorRepository.HasBooksAsync(id))
            {
                return BadRequest($"Không thể xóa Author ID {id}. Tác giả này đang có sách tham chiếu.");
            }

            var authorDelete = _authorRepository.DeleteAuthorById(id);
            if (authorDelete == null) return NotFound();
            return Ok();
        }
    }
}