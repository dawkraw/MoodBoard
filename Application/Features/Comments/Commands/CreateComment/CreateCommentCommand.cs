using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Commands.CreateComment;

public record CreateCommentCommand(string ItemId, string CommentContent) : IRequest<Comment?>;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Comment?>
{
    private readonly IMoodBoardDbContext _context;

    public CreateCommentCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.BoardItems.FirstAsync(i => i.BoardItemId == Guid.Parse(request.ItemId),
            cancellationToken);

        var newComment = new Comment
        {
            Item = item,
            Content = request.CommentContent
        };
        _context.Comments.Add(newComment);
        await _context.SaveChangesAsync(cancellationToken);
        return newComment;
    }
}