namespace SkillExtractor.Domain.Entities;

public class User
{
  public Guid Id { get; private set; }
  public string Email { get; private set; } = string.Empty;
  public string PasswordHash { get; private set; } = string.Empty;
  public string FirstName { get; private set; } = string.Empty;
  public string LastName { get; private set; } = string.Empty;
  public string Role { get; private set; } = "Employee";
  public bool IsActive { get; private set; } = true;
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; }

  // EF Core requires a parameterless constructor
  private User() { }

  public static User Create(string email, string passwordHash, string firstName, string lastName, string role = "Employee")
  {
    return new User
    {
      Id = Guid.NewGuid(),
      Email = email.ToLowerInvariant(),
      PasswordHash = passwordHash,
      FirstName = firstName,
      LastName = lastName,
      Role = role,
      IsActive = true,
      CreatedAt = DateTime.UtcNow,
    };
  }

  public void SetRole(string role) { Role = role; UpdatedAt = DateTime.UtcNow; }
  public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
  public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
}
