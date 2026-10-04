using eCommerce.Core.Domain.DTO;

namespace eCommerce.Core.ServiceContracts;
public interface IUserService
{
    Task<AuthenticationResponseDTO?> Login(LoginRequestDTO loginRequest);
    Task<AuthenticationResponseDTO?> Register(RegisterRequestDTO registerRequest);
    Task<UserDTO?> GetUserByUserId(Guid userId);
}