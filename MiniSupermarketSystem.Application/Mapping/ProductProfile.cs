namespace MiniSupermarketSystem.Application.Mapping;
using AutoMapper;
using MiniSupermarketSystem.Application.Product.Command;
using MiniSupermarketSystem.Application.Product.Dtos;
using MiniSupermarketSystem.Application.Products.Command;
using MiniSupermarketSystem.Domain.Entities;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<CreateProductCommand, Product>();

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Description,
                        opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ?
                                            "No description" :
                                            src.Description));
    }
}
