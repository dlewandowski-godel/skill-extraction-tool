using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Profile.Commands.AddSkillToEmployee;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Tests.Profile;

public class AddSkillToEmployeeCommandHandlerTests
{
  private readonly IEmployeeSkillRepository _empSkillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();
  private readonly AddSkillToEmployeeCommandHandler _sut;

  private static readonly Guid AdminId = Guid.NewGuid();
  private static readonly Guid EmployeeId = Guid.NewGuid();
  private static readonly Guid SkillId = Guid.NewGuid();

  public AddSkillToEmployeeCommandHandlerTests()
  {
    _sut = new AddSkillToEmployeeCommandHandler(
        _empSkillRepo,
        _skillRepo,
        NullLogger<AddSkillToEmployeeCommandHandler>.Instance);
  }

  [Fact]
  public async Task Handle_SkillExistsAndNoExistingRecord_AddsWithManualOverride()
  {
    // Arrange
    _skillRepo.GetByIdAsync(SkillId, Arg.Any<CancellationToken>())
        .Returns(Skill.Create(SkillId, "Python", "Programming"));
    _empSkillRepo.GetByUserAndSkillAsync(EmployeeId, SkillId, Arg.Any<CancellationToken>())
        .Returns((EmployeeSkill?)null);

    // Act
    var result = await _sut.Handle(
        new AddSkillToEmployeeCommand(AdminId, EmployeeId, SkillId, ProficiencyLevel.Advanced),
        CancellationToken.None);

    // Assert
    result.Should().Be(AddSkillResult.Ok);
    await _empSkillRepo.Received(1).AddAsync(
        Arg.Is<EmployeeSkill>(s => s.IsManualOverride && s.ProficiencyLevel == ProficiencyLevel.Advanced),
        Arg.Any<CancellationToken>());
    await _empSkillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_SkillNotInTaxonomy_ReturnsSkillNotFound()
  {
    _skillRepo.GetByIdAsync(SkillId, Arg.Any<CancellationToken>())
        .Returns((Skill?)null);

    var result = await _sut.Handle(
        new AddSkillToEmployeeCommand(AdminId, EmployeeId, SkillId, ProficiencyLevel.Beginner),
        CancellationToken.None);

    result.Should().Be(AddSkillResult.SkillNotFound);
    await _empSkillRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_EmployeeAlreadyHasSkill_UpdatesExistingRecord()
  {
    // Arrange: taxonomy skill exists, employee already has it at Beginner
    _skillRepo.GetByIdAsync(SkillId, Arg.Any<CancellationToken>())
        .Returns(Skill.Create(SkillId, "Python", "Programming"));

    var existing = EmployeeSkill.Create(
        EmployeeId, SkillId, ProficiencyLevel.Beginner, Guid.NewGuid(), DocumentType.CV);
    _empSkillRepo.GetByUserAndSkillAsync(EmployeeId, SkillId, Arg.Any<CancellationToken>())
        .Returns(existing);

    // Act: admin sets level to Expert
    var result = await _sut.Handle(
        new AddSkillToEmployeeCommand(AdminId, EmployeeId, SkillId, ProficiencyLevel.Expert),
        CancellationToken.None);

    // Assert: existing record updated, NOT a new insert
    result.Should().Be(AddSkillResult.Ok);
    existing.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
    existing.IsManualOverride.Should().BeTrue();
    await _empSkillRepo.DidNotReceive().AddAsync(Arg.Any<EmployeeSkill>(), Arg.Any<CancellationToken>());
    await _empSkillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }
}
