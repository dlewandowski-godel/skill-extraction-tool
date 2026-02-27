using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Infrastructure.Identity;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
  public void Configure(EntityTypeBuilder<ApplicationUser> builder)
  {
    builder.Property(u => u.FirstName).HasMaxLength(100);
    builder.Property(u => u.LastName).HasMaxLength(100);

    builder.HasOne(u => u.Department)
        .WithMany()
        .HasForeignKey(u => u.DepartmentId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.SetNull);
  }
}
