using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_simple.Repositories
{
    public class SQLBookAuthorRepository : IBookAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Book_Author AddBookAuthor(AddBookAuthorRequestDTO request)
        {
            var bookAuthorDomain = new Book_Author
            {
                BookId = request.BookId,
                AuthorId = request.AuthorId
            };
            _dbContext.Books_Authors.Add(bookAuthorDomain);
            _dbContext.SaveChanges();
            return bookAuthorDomain;
        }

        public async Task<bool> ExistsRelationshipAsync(int bookId, int authorId)
        {
            return await _dbContext.Books_Authors.AnyAsync(ba => ba.BookId == bookId && ba.AuthorId == authorId);
        }
    }
}