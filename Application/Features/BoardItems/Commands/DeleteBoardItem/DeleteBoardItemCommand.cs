using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BoardItems.Commands.DeleteBoardItem;

public record DeleteBoardItemCommand(string BoardItemId) : IRequest<BoardItemDto?>;

public class DeleteBoardItemCommandHandler : IRequestHandler<DeleteBoardItemCommand, BoardItemDto?>
{
    private readonly IMoodBoardDbContext _context;
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IMapper _mapper;

    public DeleteBoardItemCommandHandler(IMoodBoardDbContext context, IImageProcessingService imageProcessingService,
        IMapper mapper)
    {
        _context = context;
        _imageProcessingService = imageProcessingService;
        _mapper = mapper;
    }

    public async Task<BoardItemDto?> Handle(DeleteBoardItemCommand request, CancellationToken cancellationToken)
    {
        var itemToDelete = await _context.BoardItems.Include(i => i.Board)
            .FirstAsync(i => i.BoardItemId == Guid.Parse(request.BoardItemId), cancellationToken);
        var imageId = itemToDelete.ImageId;
        _context.BoardItems.Remove(itemToDelete);

        var changes = await _context.SaveChangesAsync(cancellationToken);
        if (changes < 1) return null;

        await _imageProcessingService.RemoveImageByUrlAsync(imageId);
        return _mapper.Map<BoardItem, BoardItemDto>(itemToDelete);
    }
}