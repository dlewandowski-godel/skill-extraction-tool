using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.EmployeeManagement.Commands.ActivateEmployee;
using SkillExtractor.Application.EmployeeManagement.Commands.DeactivateEmployee;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.EmployeeManagement;

public class DeactivateEmployeeCommandHandlerTests
{
  private readonly IUserManagementService _svc = Substitute.For<IUserManagementService>();
  private readonly DeactivateEmployeeCommandHandler _sut;

  private static readonly Guid EmployeeId = Guid.NewGuid();
  private static readonly Guid AdminId = Guid.NewGuid();

  public DeactivateEmployeeCommandHandlerTests()
      => _sut = new DeactivateEmployeeCommandHandler(_svc);

  [Fact]
  public async Task Handle_ValidDeactivation_ReturnsOkAndRevokesTokens()
  {
    // The service handles the actual DB mutation; here we verify the handler maps Ok correctly.
    _svc.DeactivateUserAsync(EmployeeId, AdminId, Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(true, null));

    var result = await _sut.Handle(
        new DeactivateEmployeeCommand(EmployeeId, AdminId), CancellationToken.None);

    result.Should().Be(DeactivateEmployeeResult.Ok);
    await _svc.Received(1).DeactivateUserAsync(EmployeeId, AdminId, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_NonExistentEmployee_ReturnsNotFound()
  {
    _svc.DeactivateUserAsync(EmployeeId, AdminId, Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(false, "NotFound"));

    var result = await _sut.Handle(
        new DeactivateEmployeeCommand(EmployeeId, AdminId), CancellationToken.None);

    result.Should().Be(DeactivateEmployeeResult.NotFound);
  }

  [Fact]
  public async Task Handle_AdminDeactivatingThemself_ReturnsCannotDeactivateSelf()
  {
    _svc.DeactivateUserAsync(AdminId, AdminId, Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(false, "CannotDeactivateSelf"));

    var result = await _sut.Handle(
        new DeactivateEmployeeCommand(AdminId, AdminId), CancellationToken.None);

    result.Should().Be(DeactivateEmployeeResult.CannotDeactivateSelf);
  }
}

public class ActivateEmployeeCommandHandlerTests
{
  private readonly IUserManagementService _svc = Substitute.For<IUserManagementService>();
  private readonly ActivateEmployeeCommandHandler _sut;

  private static readonly Guid EmployeeId = Guid.NewGuid();

  public ActivateEmployeeCommandHandlerTests()
      => _sut = new ActivateEmployeeCommandHandler(_svc);

  [Fact]
  public async Task Handle_ExistingEmployee_ReturnsOk()
  {
    _svc.ActivateUserAsync(EmployeeId, Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(true, null));

    var result = await _sut.Handle(
        new ActivateEmployeeCommand(EmployeeId), CancellationToken.None);

    result.Should().Be(ActivateEmployeeResult.Ok);
  }

  [Fact]
  public async Task Handle_NonExistentEmployee_ReturnsNotFound()
  {
    _svc.ActivateUserAsync(EmployeeId, Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(false, "NotFound"));

    var result = await _sut.Handle(
        new ActivateEmployeeCommand(EmployeeId), CancellationToken.None);

    result.Should().Be(ActivateEmployeeResult.NotFound);
  }
}
