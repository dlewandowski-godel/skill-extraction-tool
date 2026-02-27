using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Profile;
using SkillExtractor.Application.Profile.Queries.GetEmployeeProfileById;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;
using System.Reflection;

namespace SkillExtractor.Tests.Profile;

public class GetEmployeeProfileByIdQueryHandlerTests
{
  private readonly IEmployeeSkillRepository _skillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
  private readonly GetEmployeeProfileByIdQueryHandler _sut;

  private static readonly Guid EmployeeId = Guid.NewGuid();
  private static readonly Guid SkillId = Guid.NewGuid();

  public GetEmployeeProfileByIdQueryHandlerTests()
  {
    _sut = new GetEmployeeProfileByIdQueryHandler(_skillRepo, _userRepo);

    _userRepo.GetProfileInfoAsync(EmployeeId, Arg.Any<CancellationToken>())
        .Returns(new UserProfileInfo(EmployeeId, "Bob Jones", "Bob", "Jones", null, null, "Employee", true));

    _skillRepo.GetWithSkillsByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill>());
  }

  [Fact]
  public async Task Handle_ExistingEmployee_ReturnsProfile()
  {
    var result = await _sut.Handle(
        new GetEmployeeProfileByIdQuery(EmployeeId), CancellationToken.None);

    result.Should().NotBeNull();
    result!.UserId.Should().Be(EmployeeId);
    result.FullName.Should().Be("Bob Jones");
  }

  [Fact]
  public async Task Handle_NonExistentEmployee_ReturnsNull()
  {
    var unknownId = Guid.NewGuid();
    _userRepo.GetProfileInfoAsync(unknownId, Arg.Any<CancellationToken>())
        .Returns((UserProfileInfo?)null);

    var result = await _sut.Handle(
        new GetEmployeeProfileByIdQuery(unknownId), CancellationToken.None);

    result.Should().BeNull();
  }

  [Fact]
  public async Task Handle_ReturnsCorrectEmployeesSkillsOnly()
  {
    // Only skills for EmployeeId should be fetched
    await _sut.Handle(new GetEmployeeProfileByIdQuery(EmployeeId), CancellationToken.None);

    await _skillRepo.Received(1)
        .GetWithSkillsByUserAsync(EmployeeId, Arg.Any<CancellationToken>());
    await _skillRepo.DidNotReceive()
        .GetWithSkillsByUserAsync(
            Arg.Is<Guid>(id => id != EmployeeId),
            Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_WithSkills_MapsCorrectly()
  {
    var empSkill = EmployeeSkill.Create(EmployeeId, SkillId, ProficiencyLevel.Expert,
        Guid.NewGuid(), DocumentType.IFU);
    empSkill.SetManualOverrideProficiency(ProficiencyLevel.Expert);

    var taxSkill = Skill.Create(SkillId, "C#", "Programming Languages");
    typeof(EmployeeSkill)
        .GetProperty("TaxonomySkill", BindingFlags.Public | BindingFlags.Instance)!
        .SetValue(empSkill, taxSkill);

    _skillRepo.GetWithSkillsByUserAsync(EmployeeId, Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill> { empSkill });

    var result = await _sut.Handle(
        new GetEmployeeProfileByIdQuery(EmployeeId), CancellationToken.None);

    result!.Skills.Should().HaveCount(1);
    result.Skills[0].IsManualOverride.Should().BeTrue();
    result.Skills[0].ProficiencyLevel.Should().Be("Expert");
  }
}
