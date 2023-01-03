using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Queries.GetItemsForBoard;

public class GetItemsForBoardQueryValidator : AbstractValidator<GetItemsForBoardQuery>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardId;

    public GetItemsForBoardQueryValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;

        RuleFor(c => c.BoardId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardId)).WithMessage("BoardId is invalid")
            .MustAsync(BoardExists).WithMessage("Board does not exist.")
            .MustAsync(IsUserInBoard).WithMessage("You do not belong to the board.");
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
            .AsNoTracking()
            .FirstAsync(cancellationToken);
        return _loggedInUserService.UserId != null && board.ContainsMember(_loggedInUserService.UserId);
    }
}