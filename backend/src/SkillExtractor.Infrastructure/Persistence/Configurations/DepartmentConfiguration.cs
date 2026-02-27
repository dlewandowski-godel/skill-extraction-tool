using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
  public void Configure(EntityTypeBuilder<Department> builder)
  {
    builder.ToTable("departments");
    builder.HasKey(d => d.Id);
    builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
    builder.HasIndex(d => d.Name).IsUnique();
    builder.Property(d => d.CreatedAt).IsRequired();
  }
}
