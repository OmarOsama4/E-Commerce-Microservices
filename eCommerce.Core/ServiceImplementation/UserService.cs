using AutoMapper;
using eCommerce.Core.Domain.Contracts;
using eCommerce.Core.Domain.DTO;
using eCommerce.Core.Domain.Entities;
using eCommerce.Core.ServiceContracts;

namespace eCommerce.Core.ServiceImplementation;
internal class UserService(IUserRepository userRepository,
    IMapper mapper) : IUserService
{
    public async Task<AuthenticationResponseDTO?> Login(LoginRequestDTO loginRequest)
    {
        var user = await userRepository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);

        if (user is null)
            return null;

        return mapper.Map<AuthenticationResponseDTO>(user) with
        { Success = true, Token = "Token" };
    }

    public async Task<AuthenticationResponseDTO?> Register(RegisterRequestDTO registerRequest)
    {
        ApplicationUser user = mapper.Map<ApplicationUser>(registerRequest);

        var registerUser = await userRepository.AddUser(user);

        if (registerUser is null)
            return null;

        return mapper.Map<AuthenticationResponseDTO>(registerUser)
        with { Success = true, Token = "Token" };
    }
}
