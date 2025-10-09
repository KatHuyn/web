using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Triển khai các phương thức: GetAll, GetById, Add, Update, Delete
        public List<AuthorDTO> GellAllAuthors()
        {
            var allAuthorsDomain = _dbContext.Authors.ToList();
            var allAuthorDTO = new List<AuthorDTO>();
            foreach (var authorDomain in allAuthorsDomain)
            {
                allAuthorDTO.Add(new AuthorDTO()
                {
                    Id = authorDomain.Id,
                    FullName = authorDomain.FullName
                });
            }
            return allAuthorDTO;
        }

        public AuthorNoIdDTO GetAuthorById(int id)
        {
            var authorWithIdDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == id);
            if (authorWithIdDomain == null) return null;

            var authorNoIdDTO = new AuthorNoIdDTO { FullName = authorWithIdDomain.FullName };
            return authorNoIdDTO;
        }

        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorDomainModel = new Author { FullName = addAuthorRequestDTO.FullName };
            _dbContext.Authors.Add(authorDomainModel);
            _dbContext.SaveChanges();
            return addAuthorRequestDTO;
        }

        public AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (authorDomain != null)
            {
                authorDomain.FullName = authorNoIdDTO.FullName;
                _dbContext.SaveChanges();
                return authorNoIdDTO;
            }
            return null;
        }
        public Author? DeleteAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (authorDomain != null)
            {
                if (_dbContext.Books_Authors.Any(ba => ba.AuthorId == id))
                {
                    throw new InvalidOperationException($"Cannot delete Author {id}. Hãy gỡ liên kết trong Book_Author trước khi xóa.");
                }

                _dbContext.Authors.Remove(authorDomain);
                _dbContext.SaveChanges();
            }
            return authorDomain;
        }

        public async Task<bool> HasBooksAsync(int authorId)
        {
            return await Task.FromResult(_dbContext.Books_Authors.Any(ba => ba.AuthorId == authorId));
        }

        public async Task<int> GetBooksCountAsync(int authorId)
        {
            // Count the number of books associated with the author
            var count = _dbContext.Books_Authors.Count(ba => ba.AuthorId == authorId);
            return await Task.FromResult(count);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            // Check if an author with the given id exists in the database
            var exists = _dbContext.Authors.Any(a => a.Id == id);
            return await Task.FromResult(exists);
        }
    }
}