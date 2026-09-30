using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validatos;
public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequestDTO>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(temp => temp.ProductID)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(temp => temp.ProductName)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(temp => temp.Category)
            .IsInEnum().WithMessage("Invalid category.");

        RuleFor(temp => temp.UnitPrice)
            .InclusiveBetween(0, double.MaxValue).WithMessage("Unit price must be a non-negative value.");

        RuleFor(temp => temp.QuantityInStock)
            .InclusiveBetween(0, int.MaxValue).WithMessage("Quantity in stock must be a non-negative value.");
    }
}
