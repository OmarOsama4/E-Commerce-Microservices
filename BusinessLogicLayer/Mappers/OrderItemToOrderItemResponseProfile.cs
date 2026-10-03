using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Mappers;
public class OrderItemToOrderItemResponseProfile : Profile
{
    public OrderItemToOrderItemResponseProfile()
    {
        CreateMap<OrderItem, OrderItemResponseDTO>()
          .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
          .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
          .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
          .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));
    }
}
