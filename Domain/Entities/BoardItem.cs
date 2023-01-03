using System.Text.Json.Serialization;
using Domain.ValueObjects;

namespace Domain.Entities;

public class BoardItem : AuditableEntity
{
    public Guid BoardItemId { get; set; }

    public string Title { get; set; } = null!;

    public string? Note { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string ImageId { get; set; } = null!;

    [JsonConverter(typeof(CanvasVectorJsonConverter))]
    public CanvasVector Position { get; set; } = null!;

    [JsonConverter(typeof(CanvasVectorJsonConverter))]
    public CanvasVector Size { get; set; } = null!;

    public CanvasRotation Rotation { get; set; } = null!;


    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public Board Board { get; set; } = null!;
}