using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Boards.Queries.GetItemsForBoard;

public record GetItemsForBoardQuery(string BoardId) : IRequest<List<BoardItemDto>>;

public class GetItemsForBoardQueryHandler : IRequestHandler<GetItemsForBoardQuery, List<BoardItemDto>>
{
    private readonly IMoodBoardDbContext _context;
    private readonly IMapper _mapper;

    public GetItemsForBoardQueryHandler(IMoodBoardDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BoardItemDto>> Handle(GetItemsForBoardQuery request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .Where(b => b.BoardId == Guid.Parse(request.BoardId))
            .Include(b => b.Items)
            .AsNoTracking()
            .FirstAsync(cancellationToken);
        return _mapper.Map<ICollection<BoardItem>, List<BoardItemDto>>(board.Items);
    }
}