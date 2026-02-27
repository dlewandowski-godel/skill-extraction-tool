namespace SkillExtractor.Application.Interfaces;

public record TokenResult(string AccessToken, int ExpiresInSeconds);

public interface IJwtTokenService
{
  TokenResult GenerateAccessToken(Guid userId, string email, string role);
  string GenerateRefreshToken();
}
