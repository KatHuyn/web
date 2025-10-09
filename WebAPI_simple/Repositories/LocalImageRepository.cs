using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;

namespace WebAPI_simple.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
                                    IHttpContextAccessor httpContextAccessor,
                                    AppDbContext dbContext)
        {
            _webHostEnviroment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public List<Image> GetAllInfoImages()
        {
            var allImages = _dbContext.Images.ToList();
            return allImages;
        }

        public Image Upload(Image image)
        {
            // 1. Xây dựng đường dẫn file cục bộ (Local Path)
            var localFilePath = Path.Combine(_webHostEnviroment.ContentRootPath, "Images",
                $"{image.FileName}{image.FileExtension}");

            // 2. Upload file Image lên thư mục cục bộ (Images)
            using var stream = new FileStream(localFilePath, FileMode.Create);
            image.File.CopyTo(stream); // Ghi file từ bộ nhớ vào đĩa

            // 3. Xây dựng URL để truy cập file (FilePath)
            // Ví dụ: https://localhost:7159/Images/my-image.jpg
            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;

            // 4. Lưu thông tin Image vào database
            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();

            return image;
        }

        public (byte[], string, string) DownloadFile(int Id)
        {
            try
            {
                var FileById = _dbContext.Images.Where(x => x.Id == Id).FirstOrDefault();

                if (FileById == null)
                    throw new FileNotFoundException($"Image with Id {Id} not found in database.");

                // 1. Xây dựng đường dẫn file cục bộ
                var path = Path.Combine(_webHostEnviroment.ContentRootPath, "Images", $"{FileById.FileName}{FileById.FileExtension}");

                // 2. Đọc file thành mảng bytes
                var stream = File.ReadAllBytes(path);

                var fileName = FileById.FileName + FileById.FileExtension;

                // Trả về mảng bytes, ContentType, và Tên file
                return (stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}