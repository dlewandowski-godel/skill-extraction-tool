using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Profile;
using SkillExtractor.Application.Profile.Queries.GetMyProfile;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;
using System.Reflection;

namespace SkillExtractor.Tests.Profile;

public class GetMyProfileQueryHandlerTests
{
  private readonly IEmployeeSkillRepository _skillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
  private readonly GetMyProfileQueryHandler _sut;

  private static readonly Guid UserId = Guid.NewGuid();
  private static readonly Guid SkillId = Guid.NewGuid();

  public GetMyProfileQueryHandlerTests()
  {
    _sut = new GetMyProfileQueryHandler(_skillRepo, _userRepo);

    _userRepo.GetProfileInfoAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new UserProfileInfo(UserId, "Alice Smith", null));

    _skillRepo.GetWithSkillsByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill>());
  }

  [Fact]
  public async Task Handle_UserWithNoSkills_ReturnsEmptySkillsList()
  {
    var result = await _sut.Handle(new GetMyProfileQuery(UserId), CancellationToken.None);

    result.Skills.Should().BeEmpty();
    result.UserId.Should().Be(UserId);
  }

  [Fact]
  public async Task Handle_CallsRepositoryWithCorrectUserId()
  {
    await _sut.Handle(new GetMyProfileQuery(UserId), CancellationToken.None);

    await _skillRepo.Received(1).GetWithSkillsByUserAsync(UserId, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsFullNameFromUserRepository()
  {
    var result = await _sut.Handle(new GetMyProfileQuery(UserId), CancellationToken.None);

    result.FullName.Should().Be("Alice Smith");
  }

  [Fact]
  public async Task Handle_WithSkills_MapsSkillDtosCorrectly()
  {
    // Arrange: build an EmployeeSkill with TaxonomySkill set via reflection
    var empSkill = EmployeeSkill.Create(UserId, SkillId, ProficiencyLevel.Advanced,
        Guid.NewGuid(), DocumentType.CV);
    var taxSkill = Skill.Create(SkillId, "Python", "Programming Languages");

    typeof(EmployeeSkill)
        .GetProperty("TaxonomySkill", BindingFlags.Public | BindingFlags.Instance)!
        .SetValue(empSkill, taxSkill);

    _skillRepo.GetWithSkillsByUserAsync(UserId, Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill> { empSkill });

    // Act
    var result = await _sut.Handle(new GetMyProfileQuery(UserId), CancellationToken.None);

    // Assert
    result.Skills.Should().HaveCount(1);
    var skill = result.Skills[0];
    skill.SkillId.Should().Be(SkillId);
    skill.SkillName.Should().Be("Python");
    skill.Category.Should().Be("Programming Languages");
    skill.ProficiencyLevel.Should().Be("Advanced");
    skill.IsManualOverride.Should().BeFalse();
  }

  [Fact]
  public async Task Handle_SkillsWithNullNavigation_AreFilteredOut()
  {
    // If TaxonomySkill is null (shouldn't happen in practice), skip the row
    var empSkill = EmployeeSkill.Create(UserId, SkillId, ProficiencyLevel.Beginner,
        Guid.NewGuid(), DocumentType.CV);
    // TaxonomySkill is null by default

    _skillRepo.GetWithSkillsByUserAsync(UserId, Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill> { empSkill });

    var result = await _sut.Handle(new GetMyProfileQuery(UserId), CancellationToken.None);

    result.Skills.Should().BeEmpty("skills with null navigation should be filtered out");
  }
}
