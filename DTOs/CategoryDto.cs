﻿namespace E_commerce.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }             
        public string Name { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
    }
}
