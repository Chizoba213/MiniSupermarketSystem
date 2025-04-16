namespace MiniSupermarketSystem.Application.Order.Dtos;
public class CreateOrderDto
{
    public List<OrderItemDto> Items { get; set; }
}