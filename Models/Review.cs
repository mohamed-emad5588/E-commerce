using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace E_commerce.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int Rating { get; set; } 

        public string? Comment { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
