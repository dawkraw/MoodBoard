using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.AddMemberToBoard;

public class AddMemberToBoardCommandValidator : AbstractValidator<AddMemberToBoardCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardId;

    public AddMemberToBoardCommandValidator(ILoggedInUserService loggedInUserService, IMoodBoardDbContext context)
    {
        _loggedInUserService = loggedInUserService;
        _context = context;

        RuleFor(c => c.BoardId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("BoardId cannot be empty.")
            .Must(id => Guid.TryParse(id, out _boardId)).WithMessage("BoardId is invalid")
            .MustAsync(BoardExists).WithMessage("Board does not exist.")
            .MustAsync(CreatedByRequestor).WithMessage("You must be the creator of board");

        RuleFor(c => c.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username cannot be empty.")
            .MustAsync(MemberExists).WithMessage("Member does not exist.")
            .MustAsync(NotBeInTheBoard).WithMessage("This user already is a member of this board.");
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

    private async Task<bool> MemberExists(string memberUserName, CancellationToken cancellationToken)
    {
        return await _context.MoodBoardUsers.AnyAsync(u => u.UserName == memberUserName, cancellationToken);
    }

    private async Task<bool> NotBeInTheBoard(string memberUserName, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .Where(b => b.BoardId == _boardId)
            .Include(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        return board.BoardMembers.All(m => m.UserName != memberUserName);
    }
}