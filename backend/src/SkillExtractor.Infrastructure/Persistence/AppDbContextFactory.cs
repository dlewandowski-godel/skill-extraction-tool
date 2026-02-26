using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SkillExtractor.Infrastructure.Persistence;

/// <summary>
/// Used exclusively by EF Core CLI tooling (dotnet ef migrations).
/// Not used at runtime — the real context is registered via DependencyInjection.cs.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

    // Design-time connection string — only used by EF CLI tools.
    // The real connection string is injected at runtime via configuration.
    optionsBuilder
        .UseNpgsql("Host=localhost;Port=5433;Database=skillextractor;Username=skilluser;Password=changeme")
        .UseSnakeCaseNamingConvention();

    return new AppDbContext(optionsBuilder.Options);
  }
}
