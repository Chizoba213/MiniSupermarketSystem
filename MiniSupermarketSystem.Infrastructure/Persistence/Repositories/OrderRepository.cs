using Microsoft.EntityFrameworkCore;
using MiniSupermarketSystem.Domain.Entities;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

namespace MiniSupermarketSystem.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SupermarketDbContext _context;

        public OrderRepository(SupermarketDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetOrderByReferenceAsync(string reference)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.TransactionReference == reference);
        }
        public async Task UpdateOrderPaymentStatusAsync(string reference, string status)
        {
            var order = await GetOrderByReferenceAsync(reference);
            if (order != null)
            {
                order.PaymentStatus = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderByAccountNumberAsync(string accountno)
        {
            if (string.IsNullOrWhiteSpace(accountno))
            {
                throw new ArgumentException("Account number cannot be empty", nameof(accountno));
            }

            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.BankAccountNumber == accountno);
        }
    }
}
