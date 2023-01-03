using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Models;

public record BoardItemDto(string BoardItemId,
    string Title, string Note, string ImageUrl,
    [property: JsonConverter(typeof(CanvasVectorJsonConverter))]
    CanvasVector Position,
    [property: JsonConverter(typeof(CanvasVectorJsonConverter))]
    CanvasVector Size,
    float Rotation, string BoardId, List<Comment>? Comments = null);