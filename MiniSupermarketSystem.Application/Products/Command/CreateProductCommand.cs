namespace MiniSupermarketSystem.Application.Product.Command;
using MediatR;
using MiniSupermarketSystem.Application.Product.Dtos;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;
using MiniSupermarketSystem.Domain.Entities;
using Microsoft.Extensions.Logging;

public record CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; init; }
    public decimal Price { get; init; }
    public int QuantityInStock { get; init; }
    public string Description { get; init; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IProductRepository productRepository, ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting product creation for {ProductName}", request.Name);

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                QuantityInStock = request.QuantityInStock,
                Description = request.Description
            };

            await _productRepository.AddAsync(product);
            _logger.LogInformation("Successfully created product {ProductName} with ID {ProductId}",
                product.Name, product.Id);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                Description = product.Description
            };
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected error creating product {ProductName}", request.Name);
            throw;
        }
    }
}
