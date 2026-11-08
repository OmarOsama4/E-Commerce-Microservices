using AutoMapper;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Mappers;
public class UserDTOtoOrderResponseDTO : Profile
{
    public UserDTOtoOrderResponseDTO()
    {
        CreateMap<UserDTO, OrderResponseDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email));
    }
}
