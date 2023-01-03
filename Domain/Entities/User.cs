namespace Domain.Entities;

public class User : IComparable<User>
{
    public readonly IList<Board> Boards = null!;

    private User()
    {
    }

    public User(string identityId)
    {
        IdentityId = identityId;
    }

    public string IdentityId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int CompareTo(User? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(IdentityId, other.IdentityId, StringComparison.Ordinal);
    }
}