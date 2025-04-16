using System.ComponentModel.DataAnnotations;

namespace MiniSupermarketSystem.Application.Product.Dtos;
public class CreateProductDto
{
    [Required]
    public string Name { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    public string Description { get; set; }
}

