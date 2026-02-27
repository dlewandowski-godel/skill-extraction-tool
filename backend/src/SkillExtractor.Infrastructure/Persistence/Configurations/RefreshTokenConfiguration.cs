using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.HasKey(r => r.Id);

    builder.Property(r => r.Token)
        .IsRequired()
        .HasMaxLength(512);

    builder.HasIndex(r => r.Token).IsUnique();

    builder.Property(r => r.ExpiresAt).IsRequired();
    builder.Property(r => r.IsRevoked).HasDefaultValue(false);
    builder.Property(r => r.CreatedAt).IsRequired();
  }
}
