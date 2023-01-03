using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.EditBoard;

public class EditBoardCommandValidator : AbstractValidator<EditBoardCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardId;

    public EditBoardCommandValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;

        RuleFor(c => c.BoardId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardId)).WithMessage("BoardId is invalid")
            .MustAsync(BoardExists).WithMessage("Board does not exist.")
            .MustAsync(CreatedByRequestor).WithMessage("You must be the creator of board");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(20).WithMessage("Max length for Name is 200 characters.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Description cannot be empty.")
            .MaximumLength(500).WithMessage("Maximum length for Description is 500 characters.");
    }

    private async Task<bool> BoardExists(string boardId, CancellationToken cancellationToken)
    {
        return await _context.Boards.AnyAsync(b => b.BoardId == _boardId, cancellationToken);
    }

    private async Task<bool> CreatedByRequestor(string boardId, CancellationToken cancellationToken)
    {
        var board = await _context.Boards.FirstAsync(b => b.BoardId == _boardId, cancellationToken);
        return _loggedInUserService.UserId != null && board.IsCreatedBy(_loggedInUserService.UserId);
    }
}