using Microsoft.EntityFrameworkCore;
using MiniSupermarketSystem.Application.Configurations;
using MiniSupermarketSystem.Application.Services;
using MiniSupermarketSystem.BackgroundService;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;
using MiniSupermarketSystem.Domain.Interfaces.IServices;
using MiniSupermarketSystem.Infrastructure.Persistence;
using MiniSupermarketSystem.Infrastructure.Persistence.Repositories;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders(); 
builder.Logging.AddSerilog();

// Register configuration section as POCO
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Register services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryMonitor, InventoryMonitorService>();

builder.Services.AddDbContext<SupermarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register background service
builder.Services.AddHostedService<InventoryBackgroundService>();

// Build and run the host
var app = builder.Build();
app.Run();
