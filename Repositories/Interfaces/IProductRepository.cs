using E_commerce.Models;

namespace E_commerce.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // ✅ Unified: Filtering + Search + Pagination
        Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredPaginatedAsync(
            string? name,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber,
            int pageSize
        );
    }
}
