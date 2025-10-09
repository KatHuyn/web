using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public interface IBookAuthorRepository
    {
        Book_Author AddBookAuthor(AddBookAuthorRequestDTO request);
        Task<bool> ExistsRelationshipAsync(int bookId, int authorId);
    }
}