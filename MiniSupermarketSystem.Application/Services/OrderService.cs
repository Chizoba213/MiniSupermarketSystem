namespace MiniSupermarketSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniSupermarketSystem.Application.Order.Dtos;
using MiniSupermarketSystem.Domain.Dtos;
using MiniSupermarketSystem.Domain.Entities;
using MiniSupermarketSystem.Domain.Enum;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;
using MiniSupermarketSystem.Domain.Interfaces.IServices;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentService _paymentService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IPaymentService paymentService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _paymentService = paymentService;
    }

    //public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    //{
    //    // Validate products and quantities
    //    var orderDetails = new List<OrderDetail>();
    //    decimal totalAmount = 0;

    //    foreach (var item in dto.Items)
    //    {
    //        var product = await _productRepository.GetByIdAsync(item.ProductId);
    //        if (product == null)
    //            throw new ArgumentException($"Product with ID {item.ProductId} not found");

    //        if (product.QuantityInStock < item.Quantity)
    //            throw new InvalidOperationException($"Not enough stock for product {product.Name}");

    //        var orderDetail = new OrderDetail
    //        {
    //            ProductId = product.Id,
    //            Quantity = item.Quantity,
    //            UnitPrice = product.Price
    //        };

    //        orderDetails.Add(orderDetail);
    //        totalAmount += product.Price * item.Quantity;
    //    }

    //    var transactionRef = Guid.NewGuid().ToString();

    //    CreateStaticAccountResponse bankAccountResponse = await _paymentService.GenerateBankAccountForPayment(totalAmount, transactionRef);

    //    // Create order
    //    var order = new Order
    //    {
    //        TransactionReference = transactionRef,
    //        OrderDate = DateTime.UtcNow,
    //        TotalAmount = totalAmount,
    //        PaymentStatus = PaymentStatus.Pending.ToString(),
    //        BankAccountNumber = bankAccountResponse.DestinationAccountNumber,
    //        OrderDetails = orderDetails
    //    };

    //    await _orderRepository.CreateOrderAsync(order);

    //    // Update product quantities
    //    foreach (var item in dto.Items)
    //    {
    //        var product = await _productRepository.GetByIdAsync(item.ProductId);
    //        product.QuantityInStock -= item.Quantity;
    //        await _productRepository.UpdateAsync(product);
    //    }

    //    return MapToOrderDto(order);
    //}

    //public async Task<OrderDto> GetOrderByReferenceAsync(string reference)
    //{
    //    var order = await _orderRepository.GetOrderByReferenceAsync(reference);
    //    if (order == null) return null;

    //    return MapToOrderDto(order);
    //}

    //public async Task<bool> VerifyPaymentAsync(string reference)
    //{
    //    var isVerified = await _paymentService.VerifyPayment(reference);
    //    if (isVerified)
    //    {
    //        await _orderRepository.UpdateOrderPaymentStatusAsync(reference, "Completed");
    //    }
    //    return isVerified;
    //}

    private OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            TransactionReference = order.TransactionReference,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            PaymentStatus = order.PaymentStatus,
            BankAccountNumber = order.BankAccountNumber,
            OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
            {
                ProductId = od.ProductId,
                ProductName = od.Product.Name,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice
            }).ToList()
        };
    }
}