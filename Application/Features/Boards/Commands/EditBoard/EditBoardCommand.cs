using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.EditBoard;

public record EditBoardCommand(string BoardId, string Name, string Description) : IRequest<bool>;

public class EditBoardCommandHandler : IRequestHandler<EditBoardCommand, bool>
{
    private readonly IMoodBoardDbContext _context;

    public EditBoardCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditBoardCommand request, CancellationToken cancellationToken)
    {
        var boardToEdit =
            await _context.Boards.FirstAsync(b => b.BoardId == Guid.Parse(request.BoardId), cancellationToken);

        boardToEdit.Name = request.Name;
        boardToEdit.Description = request.Description;

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }
}