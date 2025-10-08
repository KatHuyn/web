using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")] // Route mặc định: /api/Books
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public BooksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            // Get Data From Database -Domain Model
            var allBooksDomain = _dbContext.Books
                .Include(b => b.Publisher) // Join với Publisher
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author) // Join với Author qua Book_Author
                .ToList();

            // Map domain models to DTOs
            var allBooksDTO = allBooksDomain.Select(Books => new BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.IsRead ? Books.Rate.Value : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                DateAdded = Books.DateAdded,
                PublisherName = Books.Publisher.Name, // Lấy tên NXB
                AuthorNames = Books.Book_Authors.Select(n => n.Author.FullName).ToList() // Lấy danh sách tên Tác giả
            }).ToList();

            // return DTOs
            return Ok(allBooksDTO);
        }
        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            // get book Domain model from Db (Find the first book matching the ID)
            var bookWithDomain = _dbContext.Books
                .Where(n => n.Id == id)
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .FirstOrDefault(); // Thay đổi thành FirstOrDefault() để lấy 1 đối tượng

            if (bookWithDomain == null)
            {
                return NotFound();
            }

            // Map Domain Model to DTOs
            var bookWithIdDTO = new BookWithAuthorAndPublisherDTO()
            {
                Id = bookWithDomain.Id,
                Title = bookWithDomain.Title,
                Description = bookWithDomain.Description,
                IsRead = bookWithDomain.IsRead,
                DateRead = bookWithDomain.DateRead,
                Rate = bookWithDomain.Rate,
                Genre = bookWithDomain.Genre,
                CoverUrl = bookWithDomain.CoverUrl,
                DateAdded = bookWithDomain.DateAdded,
                PublisherName = bookWithDomain.Publisher.Name,
                AuthorNames = bookWithDomain.Book_Authors.Select(n => n.Author.FullName).ToList()
            };

            return Ok(bookWithIdDTO);
        }
        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            // map DTO to Domain Model
            var bookDomainModel = new Book
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = addBookRequestDTO.PublisherID
            };

            // Use Domain Model to create Book
            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            // Thêm các mối quan hệ với Tác giả vào bảng Book_Author
            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = id
                };
                _dbContext.Books_Authors.Add(_book_author);
                _dbContext.SaveChanges();
            }

            return Ok(); // Có thể trả về bookDomainModel hoặc DTO đã tạo
        }
        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);

            if (bookDomain != null)
            {
                // Cập nhật thông tin sách
                bookDomain.Title = bookDTO.Title;
                bookDomain.Description = bookDTO.Description;
                bookDomain.IsRead = bookDTO.IsRead;
                bookDomain.DateRead = bookDTO.DateRead;
                bookDomain.Rate = bookDTO.Rate;
                bookDomain.Genre = bookDTO.Genre;
                bookDomain.CoverUrl = bookDTO.CoverUrl;
                bookDomain.DateAdded = bookDTO.DateAdded;
                bookDomain.PublisherID = bookDTO.PublisherID;
                _dbContext.SaveChanges();

                // Xóa mối quan hệ cũ của sách với tác giả
                var authorDomain = _dbContext.Books_Authors.Where(a => a.BookId == id).ToList();
                if (authorDomain != null)
                {
                    _dbContext.Books_Authors.RemoveRange(authorDomain);
                    _dbContext.SaveChanges();
                }

                // Thêm mối quan hệ mới
                foreach (var authorid in bookDTO.AuthorIds)
                {
                    var _book_author = new Book_Author()
                    {
                        BookId = id,
                        AuthorId = authorid
                    };
                    _dbContext.Books_Authors.Add(_book_author);
                    _dbContext.SaveChanges();
                }

                return Ok(bookDTO);
            }
            return NotFound(); // Trả về NotFound nếu không tìm thấy sách
        }
        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);

            if (bookDomain != null)
            {
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();

                // Cần xem xét xóa luôn các record liên quan trong Books_Authors nếu chưa cấu hình Cascade Delete trong DbContext
                var bookAuthors = _dbContext.Books_Authors.Where(a => a.BookId == id).ToList();
                if (bookAuthors.Any())
                {
                    _dbContext.Books_Authors.RemoveRange(bookAuthors);
                    _dbContext.SaveChanges();
                }
            }
            return Ok(); // Trả về Ok() dù có xóa hay không (tùy theo yêu cầu nghiệp vụ)
        }
    }
}