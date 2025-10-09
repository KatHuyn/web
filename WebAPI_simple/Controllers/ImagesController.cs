using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // POST: /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public ActionResult Upload([FromForm] ImageUploadRequestDTO request)
        {
            ValidateFileUpload(request); // Kiểm tra định dạng và kích thước file

            if (ModelState.IsValid)
            {
                // Convert DTO to Domain model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription,
                };

                // Sử dụng Repository để upload và lưu metadata
                _imageRepository.Upload(imageDomainModel);

                return Ok(imageDomainModel);
            }
            return BadRequest(ModelState);
        }

        // GET: /api/Images/GetAllInfoImages
        [HttpGet]
        [Route("GetAllInfoImages")]
        public IActionResult GetAllInfoImages()
        {
            var allImages = _imageRepository.GetAllInfoImages();
            return Ok(allImages);
        }

        // GET: /api/Images/Download?id={id}
        [HttpGet]
        [Route("Download")]
        public ActionResult DownloadImage(int id)
        {
            var result = _imageRepository.DownloadFile(id);
            // Trả về file, ContentType và tên file
            return File(result.Item1, result.Item2, result.Item3);
        }

        // Phương thức kiểm tra định dạng và kích thước file
        private void ValidateFileUpload(ImageUploadRequestDTO request)
        {
            var allowExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            // Kiểm tra phần mở rộng
            if (!allowExtensions.Contains(Path.GetExtension(request.File.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file extension. Only .jpg, .jpeg, .png are allowed.");
            }

            // Kiểm tra kích thước file (10MB = 10485760 bytes)
            if (request.File.Length > 10485760) // Theo hướng dẫn là <10M
            {
                ModelState.AddModelError("file", "File size too big, please upload file <10MB.");
            }
        }
    }
}