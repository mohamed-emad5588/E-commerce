namespace E_commerce.DTOs
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // optional: limit max page size
        private const int MaxPageSize = 50;
        public int ValidPageSize => PageSize > MaxPageSize ? MaxPageSize : PageSize;
    }
}
