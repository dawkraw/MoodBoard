using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class ApplicationMappings : Profile
{
    public ApplicationMappings()
    {
        CreateMap<Board, BoardListItem>()
            .ForCtorParam(nameof(BoardListItem.LastModified),
                opt => opt.MapFrom(b => new DateTimeOffset(b.LastModifiedAt).ToUnixTimeMilliseconds()));
        CreateMap<BoardItem, BoardItemDto>()
            .ForCtorParam(nameof(BoardItemDto.BoardId), opt => opt.MapFrom(i => i.Board.BoardId))
            .ForCtorParam(nameof(BoardItemDto.Rotation), opt => opt.MapFrom(i => i.Rotation.Value))
            .ForCtorParam(nameof(BoardItemDto.Comments), opt => opt.MapFrom(i => i.Comments));
    }
}