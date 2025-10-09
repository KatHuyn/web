using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddBookAuthorRequestDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public int AuthorId { get; set; }
    }
}