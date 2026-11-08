using AutoMapper;
using eCommerce.Core.Domain.DTO;
using eCommerce.Core.Domain.Entities;

namespace eCommerce.Core.Mappers;
public class ApplicationUserMappingProfile : Profile
{
    public ApplicationUserMappingProfile()
    {
        CreateMap<ApplicationUser, AuthenticationResponseDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Success, opt => opt.Ignore());

        CreateMap<RegisterRequestDTO, ApplicationUser>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()));
    }
}
