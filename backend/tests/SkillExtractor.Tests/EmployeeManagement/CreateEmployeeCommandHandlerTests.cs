using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.EmployeeManagement.Commands.CreateEmployee;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.EmployeeManagement;

public class CreateEmployeeCommandHandlerTests
{
  private readonly IUserManagementService _svc = Substitute.For<IUserManagementService>();
  private readonly CreateEmployeeCommandHandler _sut;

  private static readonly Guid NewUserId = Guid.NewGuid();
  private const string TempPwd = "abcdefgh1234A1!";

  public CreateEmployeeCommandHandlerTests()
      => _sut = new CreateEmployeeCommandHandler(_svc);

  [Fact]
  public async Task Handle_ValidRequest_ReturnsSuccessWithTempPassword()
  {
    _svc.CreateUserAsync("Jane", "Doe", "jane@test.com", "User", null, Arg.Any<CancellationToken>())
        .Returns(new CreateUserResult(true, NewUserId, TempPwd, null));

    var result = await _sut.Handle(
        new CreateEmployeeCommand("Jane", "Doe", "jane@test.com", "User", null),
        CancellationToken.None);

    result.Succeeded.Should().BeTrue();
    result.EmployeeId.Should().Be(NewUserId);
    result.TempPassword.Should().Be(TempPwd);
    result.Error.Should().BeNull();
  }

  [Fact]
  public async Task Handle_DuplicateEmail_ReturnsConflictError()
  {
    _svc.CreateUserAsync(Arg.Any<string>(), Arg.Any<string>(), "exists@test.com",
            Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
        .Returns(new CreateUserResult(false, null, null, "Email already in use."));

    var result = await _sut.Handle(
        new CreateEmployeeCommand("Jane", "Doe", "exists@test.com", "User", null),
        CancellationToken.None);

    result.Succeeded.Should().BeFalse();
    result.Error.Should().Be("Email already in use.");
    result.EmployeeId.Should().BeNull();
  }

  [Fact]
  public async Task Handle_AdminRole_DelegatesToServiceWithAdminRole()
  {
    _svc.CreateUserAsync("Bob", "Admin", "bob@test.com", "Admin", null, Arg.Any<CancellationToken>())
        .Returns(new CreateUserResult(true, Guid.NewGuid(), TempPwd, null));

    var result = await _sut.Handle(
        new CreateEmployeeCommand("Bob", "Admin", "bob@test.com", "Admin", null),
        CancellationToken.None);

    result.Succeeded.Should().BeTrue();
    await _svc.Received(1).CreateUserAsync(
        "Bob", "Admin", "bob@test.com", "Admin", null, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_TempPasswordIsReturnedFromService()
  {
    var expectedPwd = Guid.NewGuid().ToString("N")[..12] + "A1!";
    _svc.CreateUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
        .Returns(new CreateUserResult(true, Guid.NewGuid(), expectedPwd, null));

    var result = await _sut.Handle(
        new CreateEmployeeCommand("A", "B", "a@b.com", "User", null),
        CancellationToken.None);

    result.TempPassword.Should().Be(expectedPwd);
    // Verify the format satisfies complexity: has uppercase, digit, special char
    result.TempPassword!.Should().MatchRegex(@".*[A-Z].*");
    result.TempPassword.Should().MatchRegex(@".*[0-9].*");
    result.TempPassword.Should().MatchRegex(@".*[^a-zA-Z0-9].*");
  }
}
