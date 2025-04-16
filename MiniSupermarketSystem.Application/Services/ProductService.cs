//namespace MiniSupermarketSystem.Application.Services;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using MiniSupermarketSystem.Application.Product.Dtos;
//using MiniSupermarketSystem.Domain.Entities;
//using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

//public class ProductService
//{
//    private readonly IProductRepository _productRepository;

//    public ProductService(IProductRepository productRepository)
//    {
//        _productRepository = productRepository;
//    }

//    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
//    {
//        var products = await _productRepository.GetAllAsync();
//        return products.Select(p => new ProductDto
//        {
//            Id = p.Id,
//            Name = p.Name,
//            Price = p.Price,
//            QuantityInStock = p.QuantityInStock
//        });
//    }

//    public async Task<ProductDto> GetProductByIdAsync(int id)
//    {
//        var product = await _productRepository.GetByIdAsync(id);
//        if (product == null) return null;

//        return new ProductDto
//        {
//            Id = product.Id,
//            Name = product.Name,
//            Price = product.Price,
//            QuantityInStock = product.QuantityInStock
//        };
//    }

//    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
//    {
//        var product = new Product
//        {
//            Name = dto.Name,
//            Price = dto.Price,
//            QuantityInStock = dto.QuantityInStock,
//            Description = dto.Description
//        };

//        await _productRepository.AddAsync(product);

//        return new ProductDto
//        {
//            Id = product.Id,
//            Name = product.Name,
//            Price = product.Price,
//            QuantityInStock = product.QuantityInStock
//        };
//    }
//}
