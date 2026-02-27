using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillExtractor.Infrastructure.Identity;

namespace SkillExtractor.Infrastructure.Services;

public class DatabaseSeeder
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole<Guid>> _roleManager;
  private readonly IConfiguration _configuration;
  private readonly ILogger<DatabaseSeeder> _logger;

  public DatabaseSeeder(
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole<Guid>> roleManager,
      IConfiguration configuration,
      ILogger<DatabaseSeeder> logger)
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task SeedAsync()
  {
    await EnsureRolesAsync();
    await EnsureAdminAsync();
  }

  private async Task EnsureRolesAsync()
  {
    foreach (var roleName in new[] { "Admin", "User" })
    {
      if (!await _roleManager.RoleExistsAsync(roleName))
      {
        await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() });
      }
    }
  }

  private async Task EnsureAdminAsync()
  {
    var email = _configuration["Seed:AdminEmail"] ?? "admin@skillextractor.local";
    var password = _configuration["Seed:AdminPassword"] ?? "Admin@123!";

    if (await _userManager.FindByEmailAsync(email) is not null)
    {
      _logger.LogInformation("[Seeder] Admin account already exists, skipping");
      return;
    }

    var admin = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      UserName = email,
      Email = email,
      EmailConfirmed = true,
      FirstName = "Admin",
      LastName = "User",
      IsActive = true,
      CreatedAt = DateTime.UtcNow,
    };

    var result = await _userManager.CreateAsync(admin, password);
    if (!result.Succeeded)
    {
      var errors = string.Join(", ", result.Errors.Select(e => e.Description));
      throw new InvalidOperationException($"Failed to create admin user: {errors}");
    }

    await _userManager.AddToRoleAsync(admin, "Admin");
    _logger.LogInformation("[Seeder] Admin account created");
  }
}
