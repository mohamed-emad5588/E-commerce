using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public IFormFile? Image { get; set; }
}
