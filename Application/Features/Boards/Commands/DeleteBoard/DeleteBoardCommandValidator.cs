using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandValidator : AbstractValidator<DeleteBoardCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardId;

    public DeleteBoardCommandValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;

        RuleFor(c => c.BoardId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardId)).WithMessage("BoardId is invalid")
            .MustAsync(BoardExists).WithMessage("Board does not exist.")
            .MustAsync(CreatedByRequestor).WithMessage("You must be the creator of board");
    }

    private async Task<bool> BoardExists(string boardId, CancellationToken cancellationToken)
    {
        return await _context.Boards.AnyAsync(b => b.BoardId == _boardId, cancellationToken);
    }

    private async Task<bool> CreatedByRequestor(string boardId, CancellationToken cancellationToken)
    {
        var board = await _context.Boards.FirstAsync(b => b.BoardId == _boardId, cancellationToken);
        return _loggedInUserService.UserId != null && board.IsCreatedBy(_loggedInUserService.UserId);
    }
}