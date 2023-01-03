using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.RemoveMemberFromBoard;

public class RemoveMemberFromBoardCommandValidator : AbstractValidator<RemoveMemberFromBoardCommand>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private Guid _boardId;
    private User? _user;

    public RemoveMemberFromBoardCommandValidator(ILoggedInUserService loggedInUserService, IMoodBoardDbContext context)
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
            .MustAsync(BeInTheBoard).WithMessage("This member is not in this board.")
            .MustAsync(NotBeYourself).WithMessage("You cannot kick yourself!");
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

    private async Task<bool> MemberExists(string userName, CancellationToken cancellationToken)
    {
        _user = await _context.MoodBoardUsers.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        return _user is not null;
    }

    private async Task<bool> BeInTheBoard(string userName, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .Where(b => b.BoardId == _boardId)
            .Include(b => b.BoardMembers)
            .FirstAsync(cancellationToken);
        return _user != null && board.ContainsMember(_user.IdentityId);
    }

    private Task<bool> NotBeYourself(string userName, CancellationToken cancellationToken)
    {
        return Task.FromResult(_user != null && _loggedInUserService.UserId != _user.IdentityId);
    }
}