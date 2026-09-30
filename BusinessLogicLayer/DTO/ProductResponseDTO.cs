using BusinessLogicLayer.DTO.Enum;

namespace BusinessLogicLayer.DTO;
public record ProductResponseDTO(
    Guid ProductID,
    string ProductName,
    CategoryOptions Category,
    double? UnitPrice,
    int? QuantityInStock
)
{
    public ProductResponseDTO() : this(default, default, default, default, default)
    {

    }
}
