using System.Numerics;
using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Commands.CreateBoardItem;

public record CreateBoardItemCommand
    (string BoardId, string Title, string Note, string ImageUrl) : IRequest<BoardItemDto?>;

public class CreateBoardItemCommandHandler : IRequestHandler<CreateBoardItemCommand, BoardItemDto?>
{
    private readonly IMoodBoardDbContext _context;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMapper _mapper;

    public CreateBoardItemCommandHandler(IMoodBoardDbContext context, IImageProcessingService imageProcessingService,
        IMapper mapper)
    {
        _context = context;
        _imageProcessingService = imageProcessingService;
        _mapper = mapper;
    }

    public async Task<BoardItemDto?> Handle(CreateBoardItemCommand request, CancellationToken cancellationToken)
    {
        var board = await _context.Boards.FirstAsync(b => b.BoardId == Guid.Parse(request.BoardId), cancellationToken);
        var fileName = Path.GetRandomFileName();
        var image = await _imageProcessingService.UploadImageByUrlAsync(request.ImageUrl, fileName);
        if (image is null) return null;
        var newBoardItem = new BoardItem
        {
            Title = request.Title,
            Note = request.Note,
            ImageUrl = image.ImageUrl,
            ImageId = image.ImageId,
            Size = image.ImageSize,
            Position = CanvasVector.From(new Vector2(0, 0)),
            Rotation = CanvasRotation.From(0)
        };

        board.Items.Add(newBoardItem);
        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0 ? _mapper.Map<BoardItem, BoardItemDto>(newBoardItem) : null;
    }
}