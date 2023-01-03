using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.AddMemberToBoard;

public record AddMemberToBoardCommand(string UserName, string BoardId) : IRequest<User?>;

public class AddMemberToBoardCommandHandler : IRequestHandler<AddMemberToBoardCommand, User?>
{
    private readonly IMoodBoardDbContext _context;

    public AddMemberToBoardCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Handle(AddMemberToBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards.FirstAsync(b => b.BoardId == Guid.Parse(request.BoardId), cancellationToken);

        var userToAdd =
            await _context.MoodBoardUsers.FirstAsync(u => u.UserName == request.UserName, cancellationToken);

        board.BoardMembers.Add(userToAdd);
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0 ? userToAdd : null;
    }
}