﻿namespace E_commerce.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } // ← Enum كـ نص

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
