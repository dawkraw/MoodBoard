using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Commands.EditBoardItem;

public record EditBoardItemCommand(string BoardItemId, string Title, string Note) : IRequest<bool>;

public class EditBoardItemCommandHandler : IRequestHandler<EditBoardItemCommand, bool>
{
    private readonly IMoodBoardDbContext _context;

    public EditBoardItemCommandHandler(IMoodBoardDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditBoardItemCommand request, CancellationToken cancellationToken)
    {
        var itemToEdit = await _context.BoardItems.FirstAsync(b => b.BoardItemId == Guid.Parse(request.BoardItemId),
            cancellationToken);

        itemToEdit.Title = request.Title;
        itemToEdit.Note = request.Note;

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }
}