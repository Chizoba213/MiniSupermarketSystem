namespace MiniSupermarketSystem.Application.Mapping;
using AutoMapper;
using MiniSupermarketSystem.Application.Order.Dtos;
using MiniSupermarketSystem.Domain.Entities;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

        CreateMap<OrderDetail, OrderDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
    }
}