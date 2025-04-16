namespace MiniSupermarketSystem.Application.Products.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using MiniSupermarketSystem.Application.Product.Dtos;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(IProductRepository productRepository, ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {

        try
        {
            _logger.LogInformation("Fetching product with ID {ProductId}", request.Id);

            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", request.Id);
                return null; 
            }

            _logger.LogDebug("Retrieved product: {ProductDetails}", new
            {
                product.Name,
                product.Price,
                Stock = product.QuantityInStock,
                HasDescription = !string.IsNullOrEmpty(product.Description)
            });

            _logger.LogInformation("Successfully returned product {ProductName} (ID: {ProductId})",
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
            _logger.LogError(ex, "Error fetching product {ProductId}", request.Id);
            throw; 
        }
    }
}