using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required]
        [MinLength(10)]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]*$", ErrorMessage = "Title contains invalid characters.")]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }

        [Range(0, 5, ErrorMessage = "From 0 to 5")]
        public int? Rate { get; set; }

        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }

        [Required] 
        public int PublisherID { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}