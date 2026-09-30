using BusinessLogicLayer.DTO.Enum;

namespace BusinessLogicLayer.DTO;
public record ProductAddRequestDTO(
    string ProductName,
    CategoryOptions Category,
    double? UnitPrice,
    int? QuantityInStock
)
{
    public ProductAddRequestDTO() : this(default, default, default, default)
    {
        
    }
}
