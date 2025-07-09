namespace E_commerce.Models
{
    public class Order
    {
        public enum OrderStatus
        {
            Pending,      
            Processing,  
            Shipped,      
            Delivered,    
            Cancelled     
        }

        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
