namespace eCommerce.Core.Domain.DTO;
public record AuthenticationResponseDTO
(
    Guid UserId,
    string? Email,
    string? Name,
    string? Gender,
    string? Token,
    bool Success
)
{
    public AuthenticationResponseDTO() : this(default, default, default, default, default, default)
    {
    }
}
