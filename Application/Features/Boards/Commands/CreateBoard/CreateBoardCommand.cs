using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Commands.CreateBoard;

public record CreateBoardCommand(string Name, string Description) : IRequest<BoardListItem>;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, BoardListItem>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IMapper _mapper;

    public CreateBoardCommandHandler(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService,
        IMapper mapper)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;
        _mapper = mapper;
    }

    public async Task<BoardListItem> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var newBoard = new Board
        {
            Name = request.Name,
            Description = request.Description
        };
        var user = await _context.MoodBoardUsers.FirstAsync(u => u.IdentityId == _loggedInUserService.UserId,
            cancellationToken);

        newBoard.BoardMembers.Add(user);

        _context.Boards.Add(newBoard);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BoardListItem>(newBoard);
    }
}