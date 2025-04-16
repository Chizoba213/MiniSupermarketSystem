namespace MiniSupermarketSystem.API.Controllers;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MiniSupermarketSystem.Application.Orders.Command;

[ApiController]
[Route("api/webhooks")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentWebhookController> _logger;

    public PaymentWebhookController(
        IMediator mediator,
        ILogger<PaymentWebhookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("payment-notifications")]
    public async Task<IActionResult> HandlePaymentNotification()
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();
        _logger.LogInformation("Received payment notification: {RawBody}", rawBody);

        try
        {
            using var json = JsonDocument.Parse(rawBody);
            var root = json.RootElement;

            if (!root.TryGetProperty("TransactionResponseMessage", out var statusProp) ||
                !root.TryGetProperty("DestinationAccountNumber", out var referenceProp) ||
                !root.TryGetProperty("Amount", out var amountProp))
            {
                _logger.LogWarning("Missing required fields in payment notification");
                return BadRequest(new { Error = "Missing required fields: TransactionResponseMessage, DestinationAccountNumber, Amount" });
            }

            if (!amountProp.TryGetDecimal(out decimal amountPaid) || amountPaid <= 0)
            {
                _logger.LogWarning("Invalid amount value: {Amount}", amountProp.ToString());
                return BadRequest(new { Error = "Amount must be a positive decimal number" });
            }

            var statusMessage = statusProp.GetString();
            var reference = referenceProp.GetString();

            if (string.Equals(statusMessage, "Successful", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Processing successful payment for reference: {Reference}, Amount: {Amount}",
                    reference, amountPaid);

                
                await _mediator.Send(new VerifyPaymentCommand(reference, "completed", amountPaid));
                _logger.LogInformation("Successfully processed payment notification for {Reference}", reference);

                return Ok(new
                {
                    Message = "Notification processed successfully",
                    Reference = reference,
                    Amount = amountPaid
                });
            }

            _logger.LogInformation("Ignoring non-successful payment status: {Status}", statusMessage);
            return Ok(new { Message = "Non-successful status ignored", Status = statusMessage });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse payment notification JSON");
            return BadRequest(new { Error = "Invalid JSON format" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing payment notification");
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }
}