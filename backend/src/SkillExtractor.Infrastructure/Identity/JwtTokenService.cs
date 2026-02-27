using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Infrastructure.Identity;

public class JwtTokenService : IJwtTokenService
{
  private readonly JwtSettings _settings;

  public JwtTokenService(IOptions<JwtSettings> options)
  {
    _settings = options.Value;
  }

  public TokenResult GenerateAccessToken(Guid userId, string email, string role)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

    var expires = DateTime.UtcNow.AddSeconds(_settings.AccessTokenLifetimeSeconds);

    var token = new JwtSecurityToken(
        issuer: _settings.Issuer,
        audience: _settings.Audience,
        claims: claims,
        expires: expires,
        signingCredentials: credentials);

    return new TokenResult(
        new JwtSecurityTokenHandler().WriteToken(token),
        _settings.AccessTokenLifetimeSeconds);
  }

  public string GenerateRefreshToken()
  {
    var bytes = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes);
  }
}
