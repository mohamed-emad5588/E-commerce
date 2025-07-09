namespace E_commerce.DTOs
{
    public class WebsiteStatsDto
    {
        public int UsersCount { get; set; }
        public int AdminsCount { get; set; }
        public int ProductsCount { get; set; }
        public int LowStockCount { get; set; }
        public int OrdersCount { get; set; }
        public int Last7DaysOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }
}
