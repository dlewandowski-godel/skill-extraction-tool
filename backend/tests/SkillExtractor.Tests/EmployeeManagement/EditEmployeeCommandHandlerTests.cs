using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.EmployeeManagement.Commands.EditEmployee;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.EmployeeManagement;

public class EditEmployeeCommandHandlerTests
{
  private readonly IUserManagementService _svc = Substitute.For<IUserManagementService>();
  private readonly EditEmployeeCommandHandler _sut;

  private static readonly Guid EmployeeId = Guid.NewGuid();
  private static readonly Guid CallerId = Guid.NewGuid();
  private static readonly Guid DeptId = Guid.NewGuid();

  public EditEmployeeCommandHandlerTests()
      => _sut = new EditEmployeeCommandHandler(_svc);

  [Fact]
  public async Task Handle_ValidUpdate_ReturnsOk()
  {
    _svc.UpdateUserAsync(EmployeeId, "Jane", "Smith", DeptId, "User", CallerId, Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(true, null));

    var result = await _sut.Handle(
        new EditEmployeeCommand(EmployeeId, CallerId, "Jane", "Smith", DeptId, "User"),
        CancellationToken.None);

    result.Should().Be(EditEmployeeResult.Ok);
  }

  [Fact]
  public async Task Handle_NonExistentEmployee_ReturnsNotFound()
  {
    _svc.UpdateUserAsync(EmployeeId, Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<Guid?>(), Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(false, "NotFound"));

    var result = await _sut.Handle(
        new EditEmployeeCommand(EmployeeId, CallerId, "Jane", "Smith", null, "User"),
        CancellationToken.None);

    result.Should().Be(EditEmployeeResult.NotFound);
  }

  [Fact]
  public async Task Handle_AdminChangingOwnRole_ReturnsCannotChangeOwnRole()
  {
    _svc.UpdateUserAsync(EmployeeId, Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<Guid?>(), Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new UserOperationResult(false, "CannotChangeOwnRole"));

    var result = await _sut.Handle(
        new EditEmployeeCommand(EmployeeId, EmployeeId, "Jane", "Smith", null, "User"),
        CancellationToken.None);

    result.Should().Be(EditEmployeeResult.CannotChangeOwnRole);
  }
}
