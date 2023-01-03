using System.Reflection;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

public class MoodBoardDbContext : IdentityDbContext<MoodBoardUser>, IMoodBoardDbContext
{
    private readonly AuditableEntitySaveInterceptor _auditableEntitySaveInterceptor = null!;

    internal MoodBoardDbContext(DbContextOptions<MoodBoardDbContext> options)
        : base(options)
    {
    }

    public MoodBoardDbContext(DbContextOptions options, AuditableEntitySaveInterceptor auditableEntitySaveInterceptor)
        : base(options)
    {
        _auditableEntitySaveInterceptor = auditableEntitySaveInterceptor;
    }

    public DbSet<Board> Boards { get; set; } = null!;
    public DbSet<BoardItem> BoardItems { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<User> MoodBoardUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}

public class MoodBoardDbContextFactory : IDesignTimeDbContextFactory<MoodBoardDbContext>
{
    
    public MoodBoardDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MoodBoardDbContext>();
        optionsBuilder.UseSqlServer();

        return new MoodBoardDbContext(optionsBuilder.Options);
    }
}