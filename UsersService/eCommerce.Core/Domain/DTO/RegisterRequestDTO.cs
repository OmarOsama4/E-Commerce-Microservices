using eCommerce.Core.Domain.DTO.Enum;

namespace eCommerce.Core.Domain.DTO;
public record RegisterRequestDTO
(
    string? Email,
    string? Password,
    string? Name,
    GenderOptions Gender
);
