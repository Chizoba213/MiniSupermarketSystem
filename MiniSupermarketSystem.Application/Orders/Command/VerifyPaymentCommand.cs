namespace MiniSupermarketSystem.Application.Orders.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using MiniSupermarketSystem.Domain.Interfaces.IRepositories;

public record VerifyPaymentCommand(string TransactionReference, string NewStatus) : IRequest;

public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<VerifyPaymentCommandHandler> _logger;

    public VerifyPaymentCommandHandler(
        IOrderRepository orderRepository,
        ILogger<VerifyPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByReferenceAsync(request.TransactionReference);

        if (order == null)
        {
            _logger.LogWarning("Order not found for reference: {Reference}", request.TransactionReference);
            throw new KeyNotFoundException($"Order with reference {request.TransactionReference} not found");
        }

        order.PaymentStatus = request.NewStatus;
        order.PaymentDate = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);

        _logger.LogInformation("Updated payment status for {Reference} to {Status}",
            request.TransactionReference, request.NewStatus);
    }
}