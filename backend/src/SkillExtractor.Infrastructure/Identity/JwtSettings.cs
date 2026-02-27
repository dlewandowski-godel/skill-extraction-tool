namespace SkillExtractor.Infrastructure.Identity;

public class JwtSettings
{
  public const string SectionName = "Jwt";

  public string Secret { get; set; } = string.Empty;
  public string Issuer { get; set; } = "SkillExtractor";
  public string Audience { get; set; } = "SkillExtractor";
  public int AccessTokenLifetimeSeconds { get; set; } = 900; // 15 minutes
  public int RefreshTokenLifetimeDays { get; set; } = 7;
}
