using BusinessLogicLayer.DTO.Enum;

namespace BusinessLogicLayer.DTO;
public record ProductUpdateRequestDTO(
    Guid ProductID,
    string ProductName,
    CategoryOptions Category,
    double? UnitPrice,
    int? QuantityInStock
)
{
    public ProductUpdateRequestDTO() : this(default, default, default, default, default)
    {

    }
}
