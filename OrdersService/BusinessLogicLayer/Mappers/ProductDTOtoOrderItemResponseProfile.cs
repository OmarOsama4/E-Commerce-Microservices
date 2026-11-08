using AutoMapper;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Mappers;
public class ProductDTOtoOrderItemResponseProfile : Profile
{
    public ProductDTOtoOrderItemResponseProfile()
    {
        CreateMap<ProductDTO, OrderItemResponseDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
    }
}
