using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;
using SkillExtractor.Infrastructure.Services;

namespace SkillExtractor.Tests.Auth;

public class DatabaseSeederTests : IDisposable
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly DatabaseSeeder _sut;

    public DatabaseSeederTests()
    {
        var userStore = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            userStore, null, null, null, null, null, null, null, null);

        var roleStore = Substitute.For<IRoleStore<IdentityRole<Guid>>>();
        _roleManager = Substitute.For<RoleManager<IdentityRole<Guid>>>(
            roleStore, null, null, null, null);

        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(dbOptions);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Seed:AdminEmail"] = "admin@test.local",
                ["Seed:AdminPassword"] = "Admin@123!",
            })
            .Build();

        _logger = Substitute.For<ILogger<DatabaseSeeder>>();

        _sut = new DatabaseSeeder(_userManager, _roleManager, _db, _configuration, _logger);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task SeedAsync_WhenNoAdminExists_CreatesAdminUser()
    {
        // Arrange — no roles, no admin
        _roleManager.RoleExistsAsync(Arg.Any<string>()).Returns(false);
        _roleManager.CreateAsync(Arg.Any<IdentityRole<Guid>>())
            .Returns(IdentityResult.Success);

        _userManager.FindByEmailAsync("admin@test.local").Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "Admin")
            .Returns(IdentityResult.Success);

        // Act
        await _sut.SeedAsync();

        // Assert
        await _userManager.Received(1).CreateAsync(
            Arg.Is<ApplicationUser>(u => u.Email == "admin@test.local"),
            Arg.Any<string>());
        await _userManager.Received(1).AddToRoleAsync(Arg.Any<ApplicationUser>(), "Admin");
    }

    [Fact]
    public async Task SeedAsync_WhenAdminAlreadyExists_DoesNotCreateDuplicate()
    {
        // Arrange — roles exist, admin exists
        _roleManager.RoleExistsAsync(Arg.Any<string>()).Returns(true);
        _userManager.FindByEmailAsync("admin@test.local")
            .Returns(new ApplicationUser { Email = "admin@test.local" });

        // Act
        await _sut.SeedAsync();

        // Assert — admin user specifically must NOT be created
        await _userManager.DidNotReceive().CreateAsync(
            Arg.Is<ApplicationUser>(u => u.Email == "admin@test.local"),
            Arg.Any<string>());
    }

    [Fact]
    public async Task SeedAsync_WhenRolesDontExist_CreatesBothRoles()
    {
        // Arrange
        _roleManager.RoleExistsAsync(Arg.Any<string>()).Returns(false);
        _roleManager.CreateAsync(Arg.Any<IdentityRole<Guid>>())
            .Returns(IdentityResult.Success);
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        // Act
        await _sut.SeedAsync();

        // Assert — both "Admin" and "User" roles created
        await _roleManager.Received(1).CreateAsync(Arg.Is<IdentityRole<Guid>>(r => r.Name == "Admin"));
        await _roleManager.Received(1).CreateAsync(Arg.Is<IdentityRole<Guid>>(r => r.Name == "User"));
    }

    [Fact]
    public async Task SeedAsync_LogsCorrectly_WhenAdminCreatedVsSkipped()
    {
        // Arrange: no admin → should log "Admin account created"
        _roleManager.RoleExistsAsync(Arg.Any<string>()).Returns(true);
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        await _sut.SeedAsync();

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("[Seeder] Admin account created")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        // Arrange: admin exists → should log "already exists, skipping"
        _userManager.FindByEmailAsync(Arg.Any<string>())
            .Returns(new ApplicationUser { Email = "admin@test.local" });

        await _sut.SeedAsync();

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("[Seeder] Admin account already exists, skipping")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }
}
