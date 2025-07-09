using E_commerce.Data;
using E_commerce.DTOs;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Repositories.Implementations
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly AppDbContext _context;

        public StatisticsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WebsiteStatsDto> GetWebsiteStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var last7Days = now.AddDays(-7);

            var users = await _context.Users.ToListAsync();
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            return new WebsiteStatsDto
            {
                UsersCount = users.Count,
                AdminsCount = users.Count(u => u.Role == UserRole.Admin),
                ProductsCount = await _context.Products.CountAsync(),
                LowStockCount = await _context.Products.CountAsync(p => p.Stock < 5),
                OrdersCount = orders.Count,
                Last7DaysOrders = orders.Count(o => o.OrderDate >= last7Days),
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                MonthlyRevenue = orders
                    .Where(o => o.OrderDate >= startOfMonth)
                    .Sum(o => o.TotalAmount)
            };
        }
    }
}
