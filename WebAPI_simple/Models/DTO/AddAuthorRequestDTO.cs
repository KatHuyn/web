using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { set; get; }
    }
}