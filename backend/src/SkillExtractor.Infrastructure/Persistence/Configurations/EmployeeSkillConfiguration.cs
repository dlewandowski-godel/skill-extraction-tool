using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class EmployeeSkillConfiguration : IEntityTypeConfiguration<EmployeeSkill>
{
  public void Configure(EntityTypeBuilder<EmployeeSkill> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.ProficiencyLevel)
           .HasConversion<int>();

    builder.Property(e => e.SourceDocumentType)
           .HasConversion<int>();

    // Unique index on (UserId, SkillId) — one row per employee/skill pair
    builder.HasIndex(e => new { e.UserId, e.SkillId }).IsUnique();

    // Fast lookup by user
    builder.HasIndex(e => e.UserId);

    // Fast deletion of auto-extracted skills for a given user + doc type
    builder.HasIndex(e => new { e.UserId, e.SourceDocumentType, e.IsManualOverride });
  }
}
