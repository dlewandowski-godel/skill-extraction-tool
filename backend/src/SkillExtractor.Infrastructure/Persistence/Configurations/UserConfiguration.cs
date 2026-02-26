using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Email)
        .IsRequired()
        .HasMaxLength(256);

    builder.HasIndex(u => u.Email)
        .IsUnique();

    builder.Property(u => u.PasswordHash)
        .IsRequired()
        .HasMaxLength(512);

    builder.Property(u => u.FirstName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(u => u.LastName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(u => u.Role)
        .IsRequired()
        .HasMaxLength(50)
        .HasDefaultValue("Employee");

    builder.Property(u => u.IsActive)
        .HasDefaultValue(true);

    builder.Property(u => u.CreatedAt)
        .IsRequired();
  }
}
