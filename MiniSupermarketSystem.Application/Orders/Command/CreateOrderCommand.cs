namespace MiniSupermarketSystem.Application.Orders.Command;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Options;
using MiniSupermarketSystem.Application.Configurations;
using MiniSupermarketSystem.Application.Order.Dtos;
using MiniSupermarketSystem.Domain.Entities;
using MiniSupermarketSystem.Domain.Enum;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;
using MiniSupermarketSystem.Domain.Interfaces.IServices;
using MiniSupermarketSystem.Domain.Dtos;
using Microsoft.Extensions.Logging;

public record CreateOrderCommand : IRequest<OrderDto>
{
    public List<OrderItemDto> Items { get; init; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly AppSettings _settings;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IPaymentService paymentService, IOptions<AppSettings> appSettings, ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _paymentService = paymentService;
        _settings = appSettings.Value;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderDetails = new List<OrderDetail>();
        decimal totalAmount = 0;
        string transactionRef = Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("Starting order processing. Transaction: {TransactionRef}", transactionRef);

            foreach (var item in request.Items)
            {
                _logger.LogDebug("Processing product {ProductId}, quantity {Quantity}", item.ProductId, item.Quantity);

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogError("Product not found: {ProductId}", item.ProductId);
                    throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");
                }

                if (product.QuantityInStock < item.Quantity)
                {
                    _logger.LogError("Insufficient stock for {ProductName}. Requested: {Quantity}, Available: {Stock}",
                        product.Name, item.Quantity, product.QuantityInStock);
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}");
                }

                var orderDetail = new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                orderDetails.Add(orderDetail);
                totalAmount += product.Price * item.Quantity;

                _logger.LogDebug("Added {ProductName} to order. Subtotal: {Subtotal}", product.Name, product.Price * item.Quantity);
            }

            CreateStaticAccountResponse bankAccountResponse = null;
            try
            {
                bankAccountResponse = await _paymentService.GenerateBankAccountForPayment(totalAmount, transactionRef);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate payment account for amount {Amount}", totalAmount);
                if (!_settings.IsTest) throw;

                _logger.LogWarning("Using test account due to payment service failure");
            }

            string bankAccountNumber = bankAccountResponse?.DestinationAccountNumber ?? _settings.TestAccount;
            string bankName = bankAccountResponse?.DestinationBankName ?? "Test Bank";

            _logger.LogInformation("Payment account generated. Bank: {BankName}, Account: {AccountNumber}",
                bankName, bankAccountNumber);

            var order = new Order
            {
                TransactionReference = transactionRef,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                PaymentStatus = PaymentStatus.Pending.ToString(),
                BankAccountNumber = bankAccountNumber,
                OrderDetails = orderDetails
            };

            await _orderRepository.CreateOrderAsync(order);
            _logger.LogInformation("Order created successfully. Order ID: {OrderId}", order.Id);

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                product.QuantityInStock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
                _logger.LogDebug("Updated stock for {ProductId}. New quantity: {Quantity}",
                    item.ProductId, product.QuantityInStock);
            }

            return new OrderDto
            {
                TransactionReference = order.TransactionReference,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                BankName = bankAccountResponse?.DestinationBankName ?? bankName,
                BankAccountName = bankAccountResponse?.DestinationAccountName ?? string.Empty,
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
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Product validation failed in order processing");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Business rule violation in order processing");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error processing order {TransactionRef}", transactionRef);
            throw new ApplicationException("Order processing failed", ex);
        }
    }
}