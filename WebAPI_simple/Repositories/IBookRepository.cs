using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebAPI_simple.Repositories
{
    public interface IBookRepository
    {
        Task<List<BookWithAuthorAndPublisherDTO>> GetAllBooks(GetAllBooksQuery query);

        BookWithAuthorAndPublisherDTO GetBookById(int id);
        AddBookRequestDTO AddBook(AddBookRequestDTO addBookRequestDTO);
        AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO bookDTO);
        Book? DeleteBookById(int id);

        Task<bool> ExistsAsync(int id);
        Task<bool> IsTitleUniqueForPublisherAsync(string title, int publisherId);
    }
}