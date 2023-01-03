using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.RemoveMemberFromBoard;

public record RemoveMemberFromBoardCommand(string UserName, string BoardId) : IRequest<bool>;

public class RemoveMemberFromBoardCommandHandler : IRequestHandler<RemoveMemberFromBoardCommand, bool>
{
    private readonly IMoodBoardDbContext _context;

    public RemoveMemberFromBoardCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveMemberFromBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards.Where(b => b.BoardId == Guid.Parse(request.BoardId))
            .Include(b => b.BoardMembers).FirstAsync(cancellationToken);

        var member = board.BoardMembers.First(u => u.UserName == request.UserName);

        board.BoardMembers.Remove(member);

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }
}