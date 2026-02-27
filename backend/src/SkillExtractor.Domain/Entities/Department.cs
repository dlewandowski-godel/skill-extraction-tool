namespace SkillExtractor.Domain.Entities;

public class Department
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public DateTime CreatedAt { get; private set; }

  private Department() { }

  public static Department Create(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    return new Department
    {
      Id = Guid.NewGuid(),
      Name = name.Trim(),
      CreatedAt = DateTime.UtcNow,
    };
  }

  public void Rename(string newName)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(newName);
    Name = newName.Trim();
  }
}
