namespace Domain.Entities;

public class Comment : AuditableEntity
{
    public Guid CommentId { get; set; }
    public BoardItem Item { get; set; } = null!;
    public string Content { get; set; } = null!;
}