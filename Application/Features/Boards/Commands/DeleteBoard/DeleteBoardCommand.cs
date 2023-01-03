using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(string BoardId) : IRequest<bool>;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, bool>
{
    private readonly IMoodBoardDbContext _context;

    public DeleteBoardCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var boardToDelete =
            await _context.Boards.FirstAsync(b => b.BoardId == Guid.Parse(request.BoardId), cancellationToken);

        _context.Boards.Remove(boardToDelete);

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }
}