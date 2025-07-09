﻿namespace E_commerce.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
