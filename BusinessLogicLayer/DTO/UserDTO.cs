namespace BusinessLogicLayer.DTO;
public record UserDTO(
    Guid UserId,
    string? Email,
    string? Name,
    string? Gender
);
