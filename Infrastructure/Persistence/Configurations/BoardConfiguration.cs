using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.Property(b => b.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasMany(b => b.BoardMembers).WithMany(u => u.Boards);

        builder.HasMany(b => b.Items);

        builder.HasOne(b => b.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(b => b.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.Navigation(b => b.CreatedBy).AutoInclude();

        builder.Navigation(b => b.LastModifiedBy).AutoInclude();

        builder.Property(b => b.CreatedAt)
            .IsRequired();
    }
}