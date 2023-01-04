namespace Domain.Entities;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }

    public string CreatedById { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public DateTime LastModifiedAt { get; set; }
    public string LastModifiedById { get; set; } = null!;
    public User? LastModifiedBy { get; set; }

    public bool IsCreatedBy(string userId)
    {
        return CreatedBy.IdentityId == userId;
    }
}