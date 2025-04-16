namespace MiniSupermarketSystem.API.Controllers;
using System.Text.Json;
using MediatR;
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
        _logger.LogInformation($"Response from notification: {rawBody}");

        try
        {
            var json = JsonDocument.Parse(rawBody);
            var root = json.RootElement;

            var reference = root.GetProperty("transaction_reference").GetString();
            var status = root.GetProperty("status").GetString(); // "completed", "failed", etc.

            await _mediator.Send(new VerifyPaymentCommand(reference, status));

            return Ok(new { Message = "Status updated successfully" });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON payload");
            return BadRequest("Invalid JSON format");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment notification");
            return StatusCode(500, "Internal server error");
        }
    }
}