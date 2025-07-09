using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationCode { get; set; }
        public DateTime? CodeGeneratedAt { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

    
    public enum UserRole
    {
        User,
        Admin
    }
}
