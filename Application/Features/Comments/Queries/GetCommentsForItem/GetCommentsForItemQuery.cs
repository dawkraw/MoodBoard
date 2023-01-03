using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Queries.GetCommentsForItem;

public record GetCommentsForItemQuery(string BoardItemId) : IRequest<ICollection<Comment>>;

public class GetCommentsForItemQueryHandler : IRequestHandler<GetCommentsForItemQuery, ICollection<Comment>>
{
    private readonly IMoodBoardDbContext _context;

    public GetCommentsForItemQueryHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Comment>> Handle(GetCommentsForItemQuery request, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .Include(c => c.Item)
            .Where(c => c.Item.BoardItemId == Guid.Parse(request.BoardItemId))
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new Comment
            {
                CommentId = c.CommentId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                LastModifiedAt = c.LastModifiedAt,
                LastModifiedBy = c.LastModifiedBy
            })
            .ToListAsync(cancellationToken);

        return comments;
    }
}