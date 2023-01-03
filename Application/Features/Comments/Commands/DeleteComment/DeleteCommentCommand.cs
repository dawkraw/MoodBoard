using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(string CommentId) : IRequest<bool>;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IMoodBoardDbContext _context;

    public DeleteCommentCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var commentToDelete =
            await _context.Comments.FirstAsync(c => c.CommentId == Guid.Parse(request.CommentId), cancellationToken);

        _context.Comments.Remove(commentToDelete);

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }
}