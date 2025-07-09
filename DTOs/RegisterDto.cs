using E_commerce.Models;
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(3)]    
    public string Password { get; set; }

    public UserRole Role { get; set; } = UserRole.User; 
}
