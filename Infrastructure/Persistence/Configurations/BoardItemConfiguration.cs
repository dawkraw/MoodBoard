using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BoardItemConfiguration : IEntityTypeConfiguration<BoardItem>
{
    public void Configure(EntityTypeBuilder<BoardItem> builder)
    {
        builder.Property(i => i.Title)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.Note)
            .HasMaxLength(500);

        builder.Property(i => i.ImageUrl)
            .IsRequired();

        builder.Property(i => i.Position)
            .HasConversion(
                v => v.ToString(),
                v => CanvasVector.ConvertFromString(v))
            .IsRequired();

        builder.Property(i => i.Size)
            .HasConversion(
                v => v.ToString(),
                v => CanvasVector.ConvertFromString(v))
            .IsRequired();

        builder.Property(i => i.Rotation)
            .HasConversion(
                v => v.Value,
                v => CanvasRotation.From(v))
            .IsRequired();

        builder.HasOne(i => i.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(i => i.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.Navigation(i => i.CreatedBy).AutoInclude();

        builder.Navigation(i => i.LastModifiedBy).AutoInclude();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasMany(i => i.Comments)
            .WithOne(c => c.Item);
    }
}