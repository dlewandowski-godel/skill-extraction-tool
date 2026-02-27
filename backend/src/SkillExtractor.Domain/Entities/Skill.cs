namespace SkillExtractor.Domain.Entities;

/// <summary>
/// Represents a skill in the taxonomy. Loaded at startup and cached for matching.
/// </summary>
public class Skill
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public string Category { get; private set; } = string.Empty;

  /// <summary>
  /// Alternative names or abbreviations for this skill (e.g., "C#", "CSharp", "c sharp").
  /// Stored as a PostgreSQL text[] array.
  /// </summary>
  public List<string> Aliases { get; private set; } = new();

  public bool IsActive { get; private set; } = true;
  public DateTime CreatedAt { get; private set; }

  // EF Core requires parameterless constructor
  private Skill() { }

  public static Skill Create(Guid id, string name, string category, IEnumerable<string>? aliases = null)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    ArgumentException.ThrowIfNullOrWhiteSpace(category);
    return new Skill
    {
      Id = id,
      Name = name,
      Category = category,
      Aliases = aliases?.ToList() ?? new List<string>(),
      IsActive = true,
      CreatedAt = DateTime.UtcNow,
    };
  }

  public void Update(string name, string category, IEnumerable<string> aliases)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name;
    Category = category;
    Aliases = aliases.ToList();
  }

  // Allow Admin to use static factory with a known id for seeding
  public static Skill Seed(Guid id, string name, string category, params string[] aliases)
      => Create(id, name, category, aliases);

  public void Deactivate() => IsActive = false;
  public void Activate() => IsActive = true;
}
