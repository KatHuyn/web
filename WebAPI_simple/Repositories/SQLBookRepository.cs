using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;
using System.Linq;
using System; // Cần thiết cho StringComparison

namespace WebAPI_simple.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BookWithAuthorAndPublisherDTO>> GetAllBooks(GetAllBooksQuery query)
        {
            var booksQuery = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Genre))
            {
                booksQuery = booksQuery.Where(b => b.Genre.Contains(query.Genre));
            }
            if (query.IsRead.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.IsRead == query.IsRead.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    booksQuery = query.IsSortAscending ? booksQuery.OrderBy(b => b.Title) : booksQuery.OrderByDescending(b => b.Title);
                }
                else if (query.SortBy.Equals("DateAdded", StringComparison.OrdinalIgnoreCase))
                {
                    booksQuery = query.IsSortAscending ? booksQuery.OrderBy(b => b.DateAdded) : booksQuery.OrderByDescending(b => b.DateAdded);
                }
            }

            var skipResults = (query.PageNumber - 1) * query.PageSize;

            booksQuery = booksQuery.Skip(skipResults).Take(query.PageSize);

            var allBooks = await booksQuery
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead.Value : null,
                    Rate = book.IsRead ? book.Rate.Value : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                })
                .ToListAsync();

            return allBooks;
        }

        public BookWithAuthorAndPublisherDTO GetBookById(int id)
        {
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

        // Giữ lại các phương thức CRUD và Validation khác
        public AddBookRequestDTO AddBook(AddBookRequestDTO addBookRequestDTO)
        {
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

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = id
                };
                _dbContext.Books_Authors.Add(_book_author);
            }
            _dbContext.SaveChanges();
            return addBookRequestDTO;
        }

        public AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);

            if (bookDomain != null)
            {
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
            return null;
        }

        public Book? DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);

            if (bookDomain != null)
            {
                var bookAuthors = _dbContext.Books_Authors.Where(a => a.BookId == id).ToList();
                if (bookAuthors.Any())
                {
                    _dbContext.Books_Authors.RemoveRange(bookAuthors);
                    _dbContext.SaveChanges();
                }
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();
            }
            return bookDomain;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Books.AnyAsync(b => b.Id == id);
        }

        public async Task<bool> IsTitleUniqueForPublisherAsync(string title, int publisherId)
        {
            return await _dbContext.Books.AnyAsync(b => b.Title == title && b.PublisherID == publisherId);
        }
    }
}