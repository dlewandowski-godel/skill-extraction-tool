using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
  private const string RefreshTokenCookie = "refresh_token";

  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  public record LoginRequest(string Email, string Password);
  public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
  public record TokenResponse(string AccessToken, int ExpiresIn, string Role);

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    var result = await _authService.LoginAsync(request.Email, request.Password);
    if (!result.Succeeded)
      return Unauthorized(new { message = result.ErrorMessage });

    AppendRefreshCookie(result.RefreshToken!);
    return Ok(new TokenResponse(result.AccessToken!, result.ExpiresIn, result.Role!));
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request)
  {
    var result = await _authService.RegisterAsync(
        request.Email, request.Password, request.FirstName, request.LastName);

    if (!result.Succeeded)
      return BadRequest(new { message = result.ErrorMessage });

    AppendRefreshCookie(result.RefreshToken!);
    return Ok(new TokenResponse(result.AccessToken!, result.ExpiresIn, result.Role!));
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh()
  {
    var rawToken = Request.Cookies[RefreshTokenCookie];
    if (string.IsNullOrEmpty(rawToken))
      return Unauthorized(new { message = "No refresh token." });

    var result = await _authService.RefreshAsync(rawToken);
    if (!result.Succeeded)
    {
      Response.Cookies.Delete(RefreshTokenCookie);
      return Unauthorized(new { message = result.ErrorMessage });
    }

    AppendRefreshCookie(result.RefreshToken!);
    return Ok(new TokenResponse(result.AccessToken!, result.ExpiresIn, result.Role!));
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    var rawToken = Request.Cookies[RefreshTokenCookie];
    await _authService.RevokeRefreshTokenAsync(rawToken);
    Response.Cookies.Delete(RefreshTokenCookie);
    return NoContent();
  }

  private void AppendRefreshCookie(string token)
  {
    Response.Cookies.Append(RefreshTokenCookie, token, new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTimeOffset.UtcNow.AddDays(7),
    });
  }
}
