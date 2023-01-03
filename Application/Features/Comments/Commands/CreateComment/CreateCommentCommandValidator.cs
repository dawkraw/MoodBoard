using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _itemId;

    public CreateCommentCommandValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;
        RuleFor(c => c.ItemId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ItemId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _itemId)).WithMessage("ItemId is invalid")
            .MustAsync(ItemExists).WithMessage("This item does not exist.")
            .MustAsync(IsUserInBoard).WithMessage("You do not belong to the board of this item");

        RuleFor(c => c.CommentContent)
            .NotEmpty().WithMessage("Comment Content cannot be empty.")
            .MaximumLength(1000).WithMessage("Comment content cannot exceed 1000 characters.");
    }

    private async Task<bool> ItemExists(string itemId, CancellationToken cancellationToken)
    {
        return await _context.BoardItems.AnyAsync(b => b.BoardItemId == _itemId, cancellationToken);
    }

    private async Task<bool> IsUserInBoard(string itemId, CancellationToken cancellationToken)
    {
        var item = await _context.BoardItems
            .Where(i => i.BoardItemId == _itemId)
            .Include(i => i.Board)
            .ThenInclude(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        var board = item.Board;
        return _loggedInUserService.UserId != null && board.ContainsMember(_loggedInUserService.UserId);
    }
}