using MediatR;
using Microsoft.Extensions.Logging;
using MiniSupermarketSystem.Application.Product.Dtos;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

namespace MiniSupermarketSystem.Application.Product.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(IProductRepository productRepository, ILogger<GetAllProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {

        try
        {
            var products = await _productRepository.GetAllAsync();
            if (!products.Any())
            {
                _logger.LogWarning("No products found in database");
            }
            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                QuantityInStock = p.QuantityInStock,
                Description = p.Description
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during {Operation}");
            throw; 
        }
    }
}