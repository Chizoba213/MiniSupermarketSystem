namespace MiniSupermarketSystem.BackgroundService;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniSupermarketSystem.Application.Configurations;
using MiniSupermarketSystem.Domain.Interfaces.IServices;

public class InventoryBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InventoryBackgroundService> _logger;
    private readonly AppSettings _settings;

    public InventoryBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<InventoryBackgroundService> logger,
        IConfiguration configuration, IOptions<AppSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inventory Background Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var inventoryMonitor = scope.ServiceProvider.GetRequiredService<IInventoryMonitor>();

            try
            {
                _logger.LogInformation("Checking inventory levels...");
                await inventoryMonitor.CheckLowInventoryAsync(5); // Threshold of 5 units
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking inventory levels");
            }

            await Task.Delay(TimeSpan.FromMinutes(_settings.InventoryCheckIntervalMinutes), stoppingToken);
        }

        _logger.LogInformation("Inventory Background Service is stopping");
    }
}
