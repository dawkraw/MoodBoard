using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.Content)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(b => b.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(b => b.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.Navigation(c => c.CreatedBy).AutoInclude();

        builder.Navigation(c => c.LastModifiedBy).AutoInclude();

        builder.Property(c => c.CreatedAt)
            .IsRequired();
    }
}