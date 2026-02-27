using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
  public void Configure(EntityTypeBuilder<Skill> builder)
  {
    builder.HasKey(s => s.Id);

    builder.Property(s => s.Name)
           .IsRequired()
           .HasMaxLength(200);

    builder.Property(s => s.Category)
           .IsRequired()
           .HasMaxLength(100);

    // Store aliases as a PostgreSQL text[] array
    builder.Property(s => s.Aliases)
           .HasColumnType("text[]");

    builder.HasIndex(s => s.Name).IsUnique();
    builder.HasIndex(s => s.IsActive);
  }
}
