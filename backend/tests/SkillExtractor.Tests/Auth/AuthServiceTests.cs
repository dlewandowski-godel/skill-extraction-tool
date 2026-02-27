using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;
using SkillExtractor.Infrastructure.Services;

namespace SkillExtractor.Tests.Auth;

public class AuthServiceTests : IDisposable
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly AppDbContext _db;
  private readonly AuthService _sut;

  public AuthServiceTests()
  {
    var store = Substitute.For<IUserStore<ApplicationUser>>();
    _userManager = Substitute.For<UserManager<ApplicationUser>>(
        store, null, null, null, null, null, null, null, null);

    _jwtTokenService = Substitute.For<IJwtTokenService>();
    _jwtTokenService.GenerateAccessToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
        .Returns(new TokenResult("access-token-xyz", 900));
    _jwtTokenService.GenerateRefreshToken().Returns("refresh-token-abc");

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;
    _db = new AppDbContext(options);

    var jwtSettings = Options.Create(new JwtSettings
    {
      Secret = "test-secret-key-that-is-at-least-32-chars",
      RefreshTokenLifetimeDays = 7,
    });

    _sut = new AuthService(_userManager, _jwtTokenService, _db, jwtSettings);
  }

  // ── Login tests ──────────────────────────────────────────────────────────

  [Fact]
  public async Task LoginAsync_ValidCredentials_ReturnsAccessTokenAndRefreshToken()
  {
    var user = MakeUser(isActive: true);
    _userManager.FindByEmailAsync(user.Email!).Returns(user);
    _userManager.CheckPasswordAsync(user, "correct").Returns(true);
    _userManager.GetRolesAsync(user).Returns(["User"]);

    var result = await _sut.LoginAsync(user.Email!, "correct");

    result.Succeeded.Should().BeTrue();
    result.AccessToken.Should().Be("access-token-xyz");
    result.RefreshToken.Should().Be("refresh-token-abc");
    result.Role.Should().Be("User");
    result.ErrorMessage.Should().BeNull();
  }

  [Fact]
  public async Task LoginAsync_InvalidPassword_ReturnsUnauthorizedResult()
  {
    var user = MakeUser(isActive: true);
    _userManager.FindByEmailAsync(user.Email!).Returns(user);
    _userManager.CheckPasswordAsync(user, "wrong").Returns(false);

    var result = await _sut.LoginAsync(user.Email!, "wrong");

    result.Succeeded.Should().BeFalse();
    result.AccessToken.Should().BeNull();
    result.ErrorMessage.Should().Be("Invalid credentials.");
  }

  [Fact]
  public async Task LoginAsync_NonExistentEmail_ReturnsUnauthorizedResult_WithoutUserEnumeration()
  {
    _userManager.FindByEmailAsync("nobody@example.com").Returns((ApplicationUser?)null);

    var result = await _sut.LoginAsync("nobody@example.com", "anything");

    result.Succeeded.Should().BeFalse();
    // same message as invalid password — no user enumeration
    result.ErrorMessage.Should().Be("Invalid credentials.");
  }

  [Fact]
  public async Task LoginAsync_DeactivatedAccount_ReturnsUnauthorizedResult()
  {
    var user = MakeUser(isActive: false);
    _userManager.FindByEmailAsync(user.Email!).Returns(user);

    var result = await _sut.LoginAsync(user.Email!, "correct");

    result.Succeeded.Should().BeFalse();
    result.ErrorMessage.Should().Be("Account is deactivated.");
    // password check must NOT be called for inactive users
    await _userManager.DidNotReceive().CheckPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
  }

  // ── Refresh tests ────────────────────────────────────────────────────────

  [Fact]
  public async Task RefreshAsync_ValidToken_ReturnsNewAccessToken()
  {
    var userId = Guid.NewGuid();
    var user = MakeUser(isActive: true, id: userId);
    var storedToken = global::SkillExtractor.Domain.Entities.RefreshToken
        .Create(userId, "refresh-token-abc", DateTime.UtcNow.AddDays(7));
    _db.RefreshTokens.Add(storedToken);
    await _db.SaveChangesAsync();

    _userManager.FindByIdAsync(userId.ToString()).Returns(user);
    _userManager.GetRolesAsync(user).Returns(["User"]);

    var result = await _sut.RefreshAsync("refresh-token-abc");

    result.Succeeded.Should().BeTrue();
    result.AccessToken.Should().Be("access-token-xyz");
  }

  [Fact]
  public async Task RefreshAsync_ExpiredToken_ReturnsFailure()
  {
    var userId = Guid.NewGuid();
    var storedToken = global::SkillExtractor.Domain.Entities.RefreshToken
        .Create(userId, "expired-token", DateTime.UtcNow.AddDays(-1));
    _db.RefreshTokens.Add(storedToken);
    await _db.SaveChangesAsync();

    var result = await _sut.RefreshAsync("expired-token");

    result.Succeeded.Should().BeFalse();
  }

  // ── Register tests ───────────────────────────────────────────────────────

  [Fact]
  public async Task RegisterAsync_NewEmail_CreatesUserAndReturnsTokens()
  {
    _userManager.FindByEmailAsync("new@test.com").Returns((ApplicationUser?)null);
    _userManager.CreateAsync(Arg.Any<ApplicationUser>(), "Password1!")
        .Returns(IdentityResult.Success);
    _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "User")
        .Returns(IdentityResult.Success);

    var result = await _sut.RegisterAsync("new@test.com", "Password1!", "Jane", "Doe");

    result.Succeeded.Should().BeTrue();
    result.AccessToken.Should().Be("access-token-xyz");
    result.RefreshToken.Should().Be("refresh-token-abc");
    result.Role.Should().Be("User");
    result.ErrorMessage.Should().BeNull();
  }

  [Fact]
  public async Task RegisterAsync_DuplicateEmail_ReturnsFailure()
  {
    var existing = MakeUser(isActive: true);
    _userManager.FindByEmailAsync(existing.Email!).Returns(existing);

    var result = await _sut.RegisterAsync(existing.Email!, "Password1!", "Jane", "Doe");

    result.Succeeded.Should().BeFalse();
    result.ErrorMessage.Should().Be("Email is already in use.");
    await _userManager.DidNotReceive().CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
  }

  [Fact]
  public async Task RegisterAsync_WeakPassword_ReturnsIdentityErrors()
  {
    _userManager.FindByEmailAsync("weak@test.com").Returns((ApplicationUser?)null);
    _userManager.CreateAsync(Arg.Any<ApplicationUser>(), "weak")
        .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too short." }));

    var result = await _sut.RegisterAsync("weak@test.com", "weak", "Jane", "Doe");

    result.Succeeded.Should().BeFalse();
    result.ErrorMessage.Should().Contain("Password too short.");
  }

  // ── Helpers ──────────────────────────────────────────────────────────────

  private static ApplicationUser MakeUser(bool isActive, Guid? id = null) => new()
  {
    Id = id ?? Guid.NewGuid(),
    Email = "user@test.com",
    UserName = "user@test.com",
    IsActive = isActive,
  };

  public void Dispose() => _db.Dispose();
}
