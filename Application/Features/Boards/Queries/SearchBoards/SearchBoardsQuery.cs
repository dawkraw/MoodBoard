using Application.Interfaces;
using Application.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Queries.SearchBoards;

public record SearchBoardsQuery(string Query) : IRequest<IEnumerable<BoardListItem>>;

public class SearchBoardsQueryHandler : IRequestHandler<SearchBoardsQuery, IEnumerable<BoardListItem>>
{
    private readonly IMoodBoardDbContext _context;
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IMapper _mapper;

    public SearchBoardsQueryHandler(IMoodBoardDbContext context, ILoggedInUserService loggedInUserService,
        IMapper mapper)
    {
        _context = context;
        _loggedInUserService = loggedInUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardListItem>> Handle(SearchBoardsQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.MoodBoardUsers.Include(u => u.Boards).AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdentityId == _loggedInUserService.UserId, cancellationToken);
        return user is null
            ? Enumerable.Empty<BoardListItem>()
            : user.Boards.Where(b => b.Name.IndexOf(request.Query, StringComparison.OrdinalIgnoreCase) > -1)
                .OrderByDescending(b => b.LastModifiedAt)
                .Select(b => _mapper.Map<BoardListItem>(b));
    }
}