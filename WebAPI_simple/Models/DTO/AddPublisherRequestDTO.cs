using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddPublisherRequestDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Publisher Name must be at least 3 characters.")]
        [MaxLength(100, ErrorMessage = "Publisher Name cannot exceed 100 characters.")]
        public string Name { set; get; }
    }
}