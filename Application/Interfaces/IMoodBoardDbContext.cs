using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IMoodBoardDbContext
{
    DbSet<Board> Boards { get; }

    DbSet<BoardItem> BoardItems { get; }

    DbSet<Comment> Comments { get; }

    DbSet<User> MoodBoardUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}