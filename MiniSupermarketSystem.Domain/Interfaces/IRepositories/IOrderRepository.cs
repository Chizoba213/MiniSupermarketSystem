using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniSupermarketSystem.Domain.Entities;

namespace MiniSupermarketSystem.Domain.Interfaces.IRepositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByReferenceAsync(string reference);
        Task UpdateOrderPaymentStatusAsync(string reference, string status);
        Task UpdateAsync(Order order);
    }
}
