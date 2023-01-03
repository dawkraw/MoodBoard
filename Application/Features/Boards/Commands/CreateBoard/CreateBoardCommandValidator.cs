using FluentValidation;

namespace Application.Features.Boards.Commands.CreateBoard;

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(20).WithMessage("Maximum length for Name is 20 characters.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description cannot be empty.")
            .MaximumLength(150).WithMessage("Maximum length for Description is 150 characters.");
    }
}