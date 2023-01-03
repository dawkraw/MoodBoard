using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Commands.CreateBoardItem;

public class CreateBoardItemCommandValidator : AbstractValidator<CreateBoardItemCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardId;

    public CreateBoardItemCommandValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;

        RuleFor(c => c.BoardId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardId)).WithMessage("BoardId is invalid")
            .MustAsync(BoardExists).WithMessage("Board does not exist.")
            .MustAsync(IsUserInBoard).WithMessage("You do not belong to this board.");

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(20).WithMessage("Maximum length for Title is 20 characters.");

        RuleFor(c => c.Note)
            .NotEmpty().WithMessage("Note cannot be empty.")
            .MaximumLength(500).WithMessage("Maximum length for Note is 500 characters.");

        RuleFor(c => c.ImageUrl)
            .NotEmpty().WithMessage("ImageUrl cannot be empty.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("This url is invalid.");
    }

    private async Task<bool> BoardExists(string boardId, CancellationToken cancellationToken)
    {
        return await _context.Boards.AnyAsync(b => b.BoardId == _boardId, cancellationToken);
    }

    private async Task<bool> IsUserInBoard(string boardId, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .Where(b => b.BoardId == _boardId)
            .Include(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        return _loggedInUserService.UserId != null && board.ContainsMember(_loggedInUserService.UserId);
    }
}