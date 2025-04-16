using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;
using MiniSupermarketSystem.Domain.Interfaces.IServices;
using MiniSupermarketSystem.Infrastructure.Persistence;
using MiniSupermarketSystem.Infrastructure.Persistence.Repositories;
using MiniSupermarketSystem.Infrastructure.Services.Implementation;

namespace MiniSupermarketSystem.Infrastructure
{
    // DependencyInjection.cs
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SupermarketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
