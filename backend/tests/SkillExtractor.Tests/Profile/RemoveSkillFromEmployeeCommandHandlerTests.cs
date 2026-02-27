using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SkillExtractor.Application.Profile.Commands.RemoveSkillFromEmployee;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Tests.Profile;

public class RemoveSkillFromEmployeeCommandHandlerTests
{
  private readonly IEmployeeSkillRepository _empSkillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly RemoveSkillFromEmployeeCommandHandler _sut;

  private static readonly Guid AdminId = Guid.NewGuid();
  private static readonly Guid EmployeeId = Guid.NewGuid();
  private static readonly Guid SkillId = Guid.NewGuid();

  public RemoveSkillFromEmployeeCommandHandlerTests()
  {
    _sut = new RemoveSkillFromEmployeeCommandHandler(
        _empSkillRepo,
        NullLogger<RemoveSkillFromEmployeeCommandHandler>.Instance);
  }

  [Fact]
  public async Task Handle_SkillExists_RemovesSuccessfully()
  {
    var existing = EmployeeSkill.Create(
        EmployeeId, SkillId, ProficiencyLevel.Intermediate, Guid.NewGuid(), DocumentType.CV);
    _empSkillRepo.GetByUserAndSkillAsync(EmployeeId, SkillId, Arg.Any<CancellationToken>())
        .Returns(existing);

    var result = await _sut.Handle(
        new RemoveSkillFromEmployeeCommand(AdminId, EmployeeId, SkillId),
        CancellationToken.None);

    result.Should().Be(RemoveSkillResult.Ok);
    _empSkillRepo.Received(1).Remove(existing);
    await _empSkillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_SkillNotFound_ReturnsNotFound()
  {
    _empSkillRepo.GetByUserAndSkillAsync(EmployeeId, SkillId, Arg.Any<CancellationToken>())
        .Returns((EmployeeSkill?)null);

    var result = await _sut.Handle(
        new RemoveSkillFromEmployeeCommand(AdminId, EmployeeId, SkillId),
        CancellationToken.None);

    result.Should().Be(RemoveSkillResult.NotFound);
    _empSkillRepo.DidNotReceive().Remove(Arg.Any<EmployeeSkill>());
    await _empSkillRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
  }
}
