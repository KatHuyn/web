namespace WebAPI_simple.Models.DTO
{
    public class GetAllBooksQuery
    {
        public string? Genre { get; set; }
        public bool? IsRead { get; set; }

        public string? SortBy { get; set; } 
        public bool IsSortAscending { get; set; } = true;

        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10;
    }
}