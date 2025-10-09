using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;

namespace WebAPI_simple.Repositories
{
    public interface IBookRepository
    {
        List<BookWithAuthorAndPublisherDTO> GetAllBooks();
        BookWithAuthorAndPublisherDTO GetBookById(int id);
        AddBookRequestDTO AddBook(AddBookRequestDTO addBookRequestDTO);
        AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO);
        Book? DeleteBookById(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsTitleUniqueForPublisherAsync(string title, int publisherId);
    }
}