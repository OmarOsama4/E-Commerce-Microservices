using AutoMapper;
using eCommerce.Core.Domain.DTO;
using eCommerce.Core.Domain.Entities;

namespace eCommerce.Core.Mappers;
public class ApplicationUserToUserDTOProfile : Profile
{
    public ApplicationUserToUserDTOProfile()
    {
        CreateMap<ApplicationUser, UserDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));
    }
}
