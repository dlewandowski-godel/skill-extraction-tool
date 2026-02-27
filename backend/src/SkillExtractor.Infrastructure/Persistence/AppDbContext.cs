using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Identity;

namespace SkillExtractor.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<Department> Departments => Set<Department>();
  public DbSet<Document> Documents => Set<Document>();
  public DbSet<Skill> Skills => Set<Skill>();
  public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();
  public DbSet<DepartmentRequiredSkill> DepartmentRequiredSkills => Set<DepartmentRequiredSkill>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}
