using Application.Interfaces;
using Application.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Queries.GetBoardsForUser;

public record GetBoardsForUserQuery : IRequest<IEnumerable<BoardListItem>>;

public class GetBoardsForUserQueryHandler : IRequestHandler<GetBoardsForUserQuery, IEnumerable<BoardListItem>>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IMapper _mapper;

    public GetBoardsForUserQueryHandler(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService,
        IMapper mapper)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardListItem>> Handle(GetBoardsForUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _context.MoodBoardUsers.Include(u => u.Boards).AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdentityId == _loggedInUserService.UserId, cancellationToken);
        return user is null
            ? Enumerable.Empty<BoardListItem>()
            : user.Boards.OrderByDescending(b => b.LastModifiedAt)
                .Select(b => _mapper.Map<BoardListItem>(b));
    }
}