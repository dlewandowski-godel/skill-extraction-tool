namespace SkillExtractor.Domain.Entities;

public class RefreshToken
{
  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public string Token { get; private set; } = string.Empty;
  public DateTime ExpiresAt { get; private set; }
  public bool IsRevoked { get; private set; }
  public DateTime CreatedAt { get; private set; }

  private RefreshToken() { }

  public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
  {
    return new RefreshToken
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Token = token,
      ExpiresAt = expiresAt,
      IsRevoked = false,
      CreatedAt = DateTime.UtcNow,
    };
  }

  public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
  public bool IsActive => !IsRevoked && !IsExpired;

  public void Revoke() => IsRevoked = true;
}
