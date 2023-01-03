using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Queries.GetBoardItem;

public record GetBoardItemQuery(string BoardItemId) : IRequest<BoardItemDto>;

public class GetBoardItemQueryHandler : IRequestHandler<GetBoardItemQuery, BoardItemDto>
{
    private readonly IMoodBoardDbContext _context;
    private readonly IMapper _mapper;

    public GetBoardItemQueryHandler(IMoodBoardDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BoardItemDto> Handle(GetBoardItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.BoardItems.Where(i => i.BoardItemId == Guid.Parse(request.BoardItemId))
            .Include(i => i.Comments)
            .Include(i => i.Board)
            .AsNoTracking()
            .FirstAsync(cancellationToken);
        return _mapper.Map<BoardItem, BoardItemDto>(item);
    }
}