using Microsoft.AspNetCore.Identity;

namespace SkillExtractor.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public bool IsActive { get; set; } = true;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public string FullName => $"{FirstName} {LastName}".Trim();

  /// <summary>Populated in Epic 7 (employee management). Null until then.</summary>
  public string? Department { get; set; }
}
