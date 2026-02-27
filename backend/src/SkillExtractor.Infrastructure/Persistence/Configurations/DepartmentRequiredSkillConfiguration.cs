using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class DepartmentRequiredSkillConfiguration : IEntityTypeConfiguration<DepartmentRequiredSkill>
{
  public void Configure(EntityTypeBuilder<DepartmentRequiredSkill> builder)
  {
    builder.ToTable("department_required_skills");
    builder.HasKey(e => e.Id);
    builder.Property(e => e.DepartmentName).IsRequired().HasMaxLength(200);
    builder.HasOne(e => e.Skill)
           .WithMany()
           .HasForeignKey(e => e.SkillId)
           .OnDelete(DeleteBehavior.Cascade);
    builder.HasIndex(e => new { e.DepartmentName, e.SkillId }).IsUnique();
  }
}
