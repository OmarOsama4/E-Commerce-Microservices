using eCommerce.Core.Domain.DTO;
using FluentValidation;

namespace eCommerce.Core.Validators;
public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
{
    public LoginRequestValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(3).WithMessage("Password must be at least 3 characters long.");
    }
}
