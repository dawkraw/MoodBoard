using System.Numerics;
using Application.Interfaces;
using Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Commands.UpdatePlacement;

public class UpdatePlacementCommandValidator : AbstractValidator<UpdatePlacementCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardItemId;

    public UpdatePlacementCommandValidator(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;

        RuleFor(c => c.BoardItemId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardItemId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardItemId)).WithMessage("BoardItemId is invalid")
            .MustAsync(BoardItemExists).WithMessage("Item does not exist.")
            .MustAsync(IsUserInBoard).WithMessage("You do not belong to the board that contains this item.");

        RuleFor(c => c.Rotation)
            .NotNull().WithMessage("Rotation cannot be empty.")
            .Must(r => CanvasRotation.TryFrom(r, out var _)).WithMessage("Rotation is invalid. (min: 0, max: 360)");

        RuleFor(c => new {c.PositionX, c.PositionY})
            .Must(pos => CanvasVector.TryFrom(new Vector2(pos.PositionX, pos.PositionY), out var _))
            .WithMessage("Vector is invalid.");
    }

    private async Task<bool> BoardItemExists(string boardItemId, CancellationToken cancellationToken)
    {
        return await _context.BoardItems.AnyAsync(b => b.BoardItemId == _boardItemId, cancellationToken);
    }

    private async Task<bool> IsUserInBoard(string boardId, CancellationToken cancellationToken)
    {
        var item = await _context.BoardItems
            .Where(i => i.BoardItemId == _boardItemId)
            .Include(i => i.Board)
            .ThenInclude(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        var board = item.Board;
        return _loggedInUserService.UserId != null && board.ContainsMember(_loggedInUserService.UserId);
    }
}