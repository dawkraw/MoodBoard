using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Commands.DeleteComment;

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _commentId;

    public DeleteCommentCommandValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;
        RuleFor(c => c.CommentId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("CommentId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _commentId)).WithMessage("CommentId is invalid")
            .MustAsync(CommentExists).WithMessage("This item does not exist.")
            .MustAsync(IsUserInBoard).WithMessage("You do not belong to the board of this comment")
            .MustAsync(CreatedByRequestor).WithMessage("You must be the creator of this comment");
    }

    private async Task<bool> CommentExists(string commentId, CancellationToken cancellationToken)
    {
        return await _context.Comments.AnyAsync(b => b.CommentId == _commentId, cancellationToken);
    }

    private async Task<bool> IsUserInBoard(string commentId, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments
            .Where(c => c.CommentId == _commentId)
            .Include(c => c.Item)
            .ThenInclude(i => i.Board)
            .ThenInclude(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        var board = comment.Item.Board;
        return _loggedInUserService.UserId != null && board.ContainsMember(_loggedInUserService.UserId);
    }

    private async Task<bool> CreatedByRequestor(string commentId, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments.FirstAsync(b => b.CommentId == _commentId, cancellationToken);
        return _loggedInUserService.UserId != null && comment.IsCreatedBy(_loggedInUserService.UserId);
    }
}