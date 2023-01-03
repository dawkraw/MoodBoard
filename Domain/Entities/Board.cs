namespace Domain.Entities;

public class Board : AuditableEntity
{
    public Guid BoardId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public ICollection<BoardItem> Items { get; set; } = new HashSet<BoardItem>();

    public ICollection<User> BoardMembers { get; set; } = new HashSet<User>();

    public bool ContainsMember(string userId)
    {
        return CreatedBy.IdentityId == userId || BoardMembers.Any(m => m.IdentityId == userId);
    }
}