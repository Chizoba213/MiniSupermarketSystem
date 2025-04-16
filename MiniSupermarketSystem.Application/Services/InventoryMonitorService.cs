namespace MiniSupermarketSystem.Application.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;
using MiniSupermarketSystem.Domain.Interfaces.IServices;

public class InventoryMonitorService : IInventoryMonitor
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<InventoryMonitorService> _logger;

    public InventoryMonitorService(
        IProductRepository productRepository,
        ILogger<InventoryMonitorService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task CheckLowInventoryAsync(int threshold)
    {
        var products = await _productRepository.GetAllAsync();
        var lowStockProducts = products.Where(p => p.QuantityInStock <= threshold).ToList();

        if (lowStockProducts.Any())
        {
            foreach (var product in lowStockProducts)
            {
                _logger.LogWarning($"Low stock alert: Product {product.Name} has only {product.QuantityInStock} units left (threshold: {threshold})");
            }
        }
    }
}