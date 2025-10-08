using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Implement GetAllBooks
        public List<BookWithAuthorAndPublisherDTO> GetAllBooks()
        {
            // Logic lấy dữ liệu từ DbContext và map sang DTO (như đã làm ở Phần 3 - Action GetAll())
            var allBooks = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .Select(Books => new BookWithAuthorAndPublisherDTO()
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
                    PublisherName = Books.Publisher.Name,
                    AuthorNames = Books.Book_Authors.Select(n => n.Author.FullName).ToList()
                }).ToList();

            return allBooks;
        }

        // Implement GetBookById
        public BookWithAuthorAndPublisherDTO GetBookById(int id)
        {
            // Logic lấy dữ liệu từ DbContext và map sang DTO (như đã làm ở Phần 3 - Action GetBookById)
            var bookWithDomain = _dbContext.Books
                .Where(n => n.Id == id)
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author);

            var bookWithIdDTO = bookWithDomain.Select(book => new BookWithAuthorAndPublisherDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.DateRead,
                Rate = book.Rate,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).FirstOrDefault();

            return bookWithIdDTO;
        }

        // Implement AddBook
        public AddBookRequestDTO AddBook(AddBookRequestDTO addBookRequestDTO)
        {
            // Logic mapping DTO to Domain Model và thêm vào CSDL (như đã làm ở Phần 3 - Action AddBook)
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

            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

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

            return addBookRequestDTO;
        }

        // Implement UpdateBookById
        public AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO)
        {
            // Logic cập nhật thông tin sách (như đã làm ở Phần 3 - Action UpdateBookById)
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

                // Xóa và Thêm mối quan hệ tác giả (như đã làm ở Phần 3 - Action UpdateBookById)
                var authorDomain = _dbContext.Books_Authors.Where(a => a.BookId == id).ToList();
                if (authorDomain != null)
                {
                    _dbContext.Books_Authors.RemoveRange(authorDomain);
                    _dbContext.SaveChanges();
                }

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

                return bookDTO;
            }
            return null; // Trả về null nếu không tìm thấy
        }

        // Implement DeleteBookById
        public Book? DeleteBookById(int id)
        {
            // Logic xóa sách (như đã làm ở Phần 3 - Action DeleteBookById)
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);

            if (bookDomain != null)
            {
                // Xóa các record liên quan trong Books_Authors (nếu cần)
                var bookAuthors = _dbContext.Books_Authors.Where(a => a.BookId == id).ToList();
                if (bookAuthors.Any())
                {
                    _dbContext.Books_Authors.RemoveRange(bookAuthors);
                    _dbContext.SaveChanges();
                }

                // Xóa sách
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();
            }
            return bookDomain;
        }
    }
}