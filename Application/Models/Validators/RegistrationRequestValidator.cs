using FluentValidation;

namespace Application.Models.Validators;

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator()
    {
        RuleFor(r => r.UserName)
            .MaximumLength(100)
            .NotEmpty().WithMessage("test1");

        RuleFor(r => r.Password)
            .MinimumLength(8)
            .NotEmpty().WithMessage("test2");
    }
}