namespace MiniSupermarketSystem.Application.Products.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using MiniSupermarketSystem.Application.Product.Dtos;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

public record UpdateProductCommand : IRequest<ProductDto>
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public int? QuantityInStock { get; init; }
    public string? Description { get; init; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductCommand> _logger;

    public UpdateProductCommandHandler(IProductRepository productRepository, ILogger<UpdateProductCommand> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to update product with ID {ProductId}", request.Id);

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogWarning("Update failed: Product {ProductId} not found", request.Id);
                throw new KeyNotFoundException($"Product with ID {request.Id} not found");
            }

            bool hasChanges = false;

            if (request.Name != null && product.Name != request.Name)
            {
                _logger.LogDebug("Updating name from '{OldName}' to '{NewName}'", product.Name, request.Name);
                product.Name = request.Name;
                hasChanges = true;
            }

            if (request.Price.HasValue && product.Price != request.Price.Value)
            {
                _logger.LogDebug("Updating price from {OldPrice} to {NewPrice}", product.Price, request.Price.Value);
                product.Price = request.Price.Value;
                hasChanges = true;
            }

            if (request.QuantityInStock.HasValue && product.QuantityInStock != request.QuantityInStock.Value)
            {
                _logger.LogDebug("Updating stock from {OldStock} to {NewStock}",
                    product.QuantityInStock, request.QuantityInStock.Value);
                product.QuantityInStock = request.QuantityInStock.Value;
                hasChanges = true;
            }

            if (request.Description != null && product.Description != request.Description)
            {
                _logger.LogDebug("Updating description");
                product.Description = request.Description;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                _logger.LogInformation("No changes detected for product {ProductId}", request.Id);
            }
            else
            {
                await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Successfully updated product {ProductId}", request.Id);
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                Description = product.Description
            };
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Product update failed - product not found");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating product {ProductId}", request.Id);
            throw;
        }
    }
}