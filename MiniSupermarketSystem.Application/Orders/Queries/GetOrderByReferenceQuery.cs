using MediatR;
using Microsoft.Extensions.Logging;
using MiniSupermarketSystem.Application.Order.Dtos;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

namespace MiniSupermarketSystem.Application.Orders.Queries;
public record GetOrderByReferenceQuery(string Reference) : IRequest<OrderDto>;

public class GetOrderByReferenceQueryHandler : IRequestHandler<GetOrderByReferenceQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrderByReferenceQueryHandler> _logger;

    public GetOrderByReferenceQueryHandler(IOrderRepository orderRepository, ILogger<GetOrderByReferenceQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(GetOrderByReferenceQuery request, CancellationToken cancellationToken)
    {

        try
        {
            var order = await _orderRepository.GetOrderByReferenceAsync(request.Reference);

            if (order == null)
            {
                _logger.LogWarning("Order not found for reference: {OrderReference}", request.Reference);
                return null;
            }

            var orderDto = new OrderDto
            {
                TransactionReference = order.TransactionReference,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                BankAccountNumber = order.BankAccountNumber,
                AmountPaid = order.AmountPaid,
                PaymentDate = order.PaymentDate,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name, 
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            return orderDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderReference}", request.Reference);
            throw;
        }
    }
}