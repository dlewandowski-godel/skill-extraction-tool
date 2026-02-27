using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly AppDbContext _db;
  private readonly JwtSettings _jwtSettings;

  public AuthService(
      UserManager<ApplicationUser> userManager,
      IJwtTokenService jwtTokenService,
      AppDbContext db,
      IOptions<JwtSettings> jwtSettings)
  {
    _userManager = userManager;
    _jwtTokenService = jwtTokenService;
    _db = db;
    _jwtSettings = jwtSettings.Value;
  }

  public async Task<RegisterResult> RegisterAsync(string email, string password, string firstName, string lastName)
  {
    var existing = await _userManager.FindByEmailAsync(email);
    if (existing is not null)
      return new RegisterResult(false, null, 0, null, "Email is already in use.");

    var user = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      UserName = email,
      Email = email,
      FirstName = firstName,
      LastName = lastName,
      IsActive = true,
      CreatedAt = DateTime.UtcNow,
    };

    var createResult = await _userManager.CreateAsync(user, password);
    if (!createResult.Succeeded)
    {
      var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
      return new RegisterResult(false, null, 0, null, errors);
    }

    await _userManager.AddToRoleAsync(user, "User");

    var token = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, "User");
    var refreshToken = await CreateRefreshTokenAsync(user.Id);

    return new RegisterResult(true, token.AccessToken, token.ExpiresInSeconds, "User", null)
    {
      RefreshToken = refreshToken
    };
  }

  public async Task<LoginResult> LoginAsync(string email, string password)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user is null || !user.IsActive)
      return new LoginResult(false, null, 0, null, "Invalid credentials.");

    if (!await _userManager.CheckPasswordAsync(user, password))
      return new LoginResult(false, null, 0, null, "Invalid credentials.");

    var roles = await _userManager.GetRolesAsync(user);
    var role = roles.FirstOrDefault() ?? "User";

    var token = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, role);
    var refreshToken = await CreateRefreshTokenAsync(user.Id);

    return new LoginResult(true, token.AccessToken, token.ExpiresInSeconds, role, null)
    {
      RefreshToken = refreshToken
    };
  }

  public async Task<RefreshResult> RefreshAsync(string rawRefreshToken)
  {
    var stored = await _db.RefreshTokens
        .FirstOrDefaultAsync(t => t.Token == rawRefreshToken);

    if (stored is null || !stored.IsActive)
      return new RefreshResult(false, null, 0, null, "Refresh token expired or revoked.");

    stored.Revoke();

    var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
    if (user is null || !user.IsActive)
      return new RefreshResult(false, null, 0, null, "User not found or inactive.");

    var roles = await _userManager.GetRolesAsync(user);
    var role = roles.FirstOrDefault() ?? "User";

    var token = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, role);
    var newRefresh = await CreateRefreshTokenAsync(user.Id);
    await _db.SaveChangesAsync();

    return new RefreshResult(true, token.AccessToken, token.ExpiresInSeconds, role, null)
    {
      RefreshToken = newRefresh
    };
  }

  public async Task RevokeRefreshTokenAsync(string? rawRefreshToken)
  {
    if (string.IsNullOrEmpty(rawRefreshToken)) return;

    var stored = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == rawRefreshToken);
    if (stored is not null)
    {
      stored.Revoke();
      await _db.SaveChangesAsync();
    }
  }

  private async Task<string> CreateRefreshTokenAsync(Guid userId)
  {
    var raw = _jwtTokenService.GenerateRefreshToken();
    var expiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
    _db.RefreshTokens.Add(RefreshToken.Create(userId, raw, expiry));
    await _db.SaveChangesAsync();
    return raw;
  }
}
