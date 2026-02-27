using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SkillExtractor.Application.Profile.Commands.ChangeProficiency;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Tests.Profile;

public class ChangeProficiencyCommandHandlerTests
{
  private readonly IEmployeeSkillRepository _empSkillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly ChangeProficiencyCommandHandler _sut;

  private static readonly Guid AdminId = Guid.NewGuid();
  private static readonly Guid EmployeeId = Guid.NewGuid();
  private static readonly Guid SkillId = Guid.NewGuid();

  public ChangeProficiencyCommandHandlerTests()
  {
    _sut = new ChangeProficiencyCommandHandler(
        _empSkillRepo,
        NullLogger<ChangeProficiencyCommandHandler>.Instance);
  }

  [Fact]
  public async Task Handle_SkillExists_UpdatesProficiencyAndSetsManualOverride()
  {
    var existing = EmployeeSkill.Create(
        EmployeeId, SkillId, ProficiencyLevel.Beginner, Guid.NewGuid(), DocumentType.CV);
    _empSkillRepo.GetByUserAndSkillAsync(EmployeeId, SkillId, Arg.Any<CancellationToken>())
        .Returns(existing);

    var result = await _sut.Handle(
        new ChangeProficiencyCommand(AdminId, EmployeeId, SkillId, ProficiencyLevel.Expert),
        CancellationToken.None);

    result.Should().Be(ChangeProficiencyResult.Ok);
    existing.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
    existing.IsManualOverride.Should().BeTrue();
    await _empSkillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_SkillNotFound_ReturnsNotFound()
  {
    _empSkillRepo.GetByUserAndSkillAsync(EmployeeId, SkillId, Arg.Any<CancellationToken>())
        .Returns((EmployeeSkill?)null);

    var result = await _sut.Handle(
        new ChangeProficiencyCommand(AdminId, EmployeeId, SkillId, ProficiencyLevel.Advanced),
        CancellationToken.None);

    result.Should().Be(ChangeProficiencyResult.NotFound);
    await _empSkillRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
  }
}
