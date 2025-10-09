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
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublisherRepository _publisherRepository;

        public PublishersController(AppDbContext dbContext, IPublisherRepository publisherRepository)
        {
            _dbContext = dbContext;
            _publisherRepository = publisherRepository;
        }

        // Triển khai các Action tương ứng với IPublisherRepository
        [HttpGet("get-all-publisher")]
        public IActionResult GetAllPublisher()
        {
            var allPublishers = _publisherRepository.GetAllPublishers();
            return Ok(allPublishers);
        }

        [HttpGet("get-publisher-by-id")]
        public IActionResult GetPublisherById(int id)
        {
            var publisherWithId = _publisherRepository.GetPublisherById(id);
            if (publisherWithId == null) return NotFound();
            return Ok(publisherWithId);
        }

        [HttpPost("add-publisher")]
        [ValidateModel]
        public async Task<ActionResult> AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            if (await _publisherRepository.IsNameUniqueAsync(addPublisherRequestDTO.Name))
            {
                ModelState.AddModelError(nameof(addPublisherRequestDTO.Name), "Publisher Name already exists.");
                return BadRequest(ModelState);
            }

            var publisherAdd = _publisherRepository.AddPublisher(addPublisherRequestDTO);
            return Ok(publisherAdd);
        }

        [HttpPut("update-publisher-by-id/{id}")]
        [ValidateModel]
        public IActionResult UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherDTO)
        {
            var publisherUpdate = _publisherRepository.UpdatePublisherById(id, publisherDTO);
            if (publisherUpdate == null) return NotFound();
            return Ok(publisherUpdate);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public async Task<ActionResult> DeletePublisherById(int id)
        {
            if (await _publisherRepository.HasBooksAsync(id))
            {
                // Báo lỗi 400 Bad Request
                return BadRequest($"Cannot delete Publisher ID {id}. There are books referencing it. Consider setting cascade delete rules in the database or removing the books first.");
            }

            var publisherDelete = _publisherRepository.DeletePublisherById(id);
            if (publisherDelete == null) return NotFound();
            return Ok();
        }
    }
}