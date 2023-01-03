using System.Numerics;
using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Commands.UpdatePlacement;

public record UpdatePlacementCommand(string BoardItemId, float PositionX, float PositionY, float Width, float Height,
    float Rotation) : IRequest<BoardItemDto?>;

public class UpdatePlacementCommandHandler : IRequestHandler<UpdatePlacementCommand, BoardItemDto?>
{
    private readonly IMoodBoardDbContext _context;
    private readonly IMapper _mapper;

    public UpdatePlacementCommandHandler(IMoodBoardDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BoardItemDto?> Handle(UpdatePlacementCommand request, CancellationToken cancellationToken)
    {
        var itemToEdit = await _context.BoardItems.Include(i => i.Board)
            .FirstAsync(b => b.BoardItemId == Guid.Parse(request.BoardItemId), cancellationToken);

        itemToEdit.Position = CanvasVector.From(new Vector2(request.PositionX, request.PositionY));
        itemToEdit.Size = CanvasVector.From(new Vector2(request.Width, request.Height));
        itemToEdit.Rotation = CanvasRotation.From(request.Rotation);

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0 ? _mapper.Map<BoardItem, BoardItemDto>(itemToEdit) : null;
    }
}