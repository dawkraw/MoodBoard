using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Queries.GetCommentsForItem;

public class GetCommentsForItemValidator : AbstractValidator<GetCommentsForItemQuery>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;

    private Guid _boardItemId;

    public GetCommentsForItemValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;
        RuleFor(c => c.BoardItemId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardItemId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardItemId)).WithMessage("BoardItemId is invalid")
            .MustAsync(BoardItemExists).WithMessage("Item does not exist.")
            .MustAsync(IsUserInBoard).WithMessage("You do not belong to the board.");
    }

    private async Task<bool> BoardItemExists(string boardItemId, CancellationToken cancellationToken)
    {
        return await _context.BoardItems.AnyAsync(b => b.BoardItemId == _boardItemId, cancellationToken);
    }

    private async Task<bool> IsUserInBoard(string boardItemId, CancellationToken cancellationToken)
    {
        var item = await _context.BoardItems
            .Where(i => i.BoardItemId == _boardItemId)
            .Include(i => i.Board)
            .ThenInclude(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        var board = item.Board;
        return _loggedInUserService.UserId != null && board.ContainsMember(_loggedInUserService.UserId);
    }
}