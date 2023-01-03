using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Queries.GetBoard;

public record GetBoardQuery(string BoardId) : IRequest<Board>;

public class GetBoardQueryHandler : IRequestHandler<GetBoardQuery, Board>
{
    private readonly IMoodBoardDbContext _context;

    public GetBoardQueryHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<Board> Handle(GetBoardQuery request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .Where(b => b.BoardId == Guid.Parse(request.BoardId))
            .Include(b => b.BoardMembers)
            .AsNoTracking()
            .FirstAsync(cancellationToken);
        return board;
    }
}