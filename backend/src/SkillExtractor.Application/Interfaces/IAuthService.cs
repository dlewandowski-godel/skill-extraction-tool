namespace SkillExtractor.Application.Interfaces;

public record LoginResult(bool Succeeded, string? AccessToken, int ExpiresIn, string? Role, string? ErrorMessage)
{
  public string? RefreshToken { get; init; }
}

public record RefreshResult(bool Succeeded, string? AccessToken, int ExpiresIn, string? Role, string? ErrorMessage)
{
  public string? RefreshToken { get; init; }
}

public record RegisterResult(bool Succeeded, string? AccessToken, int ExpiresIn, string? Role, string? ErrorMessage)
{
  public string? RefreshToken { get; init; }
}

public interface IAuthService
{
  Task<LoginResult> LoginAsync(string email, string password);
  Task<RegisterResult> RegisterAsync(string email, string password, string firstName, string lastName);
  Task<RefreshResult> RefreshAsync(string refreshToken);
  Task RevokeRefreshTokenAsync(string? refreshToken);
}
