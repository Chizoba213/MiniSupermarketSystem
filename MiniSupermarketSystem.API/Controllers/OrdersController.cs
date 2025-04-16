using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSupermarketSystem.Application.Order.Dtos;
using MiniSupermarketSystem.Application.Orders.Command;
using MiniSupermarketSystem.Application.Orders.Queries;

namespace MiniSupermarketSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderCommand command)
        {
            try
            {
                var order = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetOrder), new { reference = order.TransactionReference }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{reference}")]
        public async Task<ActionResult<OrderDto>> GetOrder(string reference)
        {
            var order = await _mediator.Send(new GetOrderByReferenceQuery(reference));
            if (order == null) return NotFound();
            return Ok(order);
        }

        //[HttpPost("{reference}/verify")]
        //public async Task<ActionResult<bool>> VerifyPayment(string reference)
        //{
        //    var isVerified = await _mediator.Send(new VerifyPaymentCommand(reference));
        //    return Ok(isVerified);
        //}
    }
}
