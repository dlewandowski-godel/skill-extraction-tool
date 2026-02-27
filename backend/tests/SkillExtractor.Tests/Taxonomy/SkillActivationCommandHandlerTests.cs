using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Taxonomy.Commands.ActivateSkill;
using SkillExtractor.Application.Taxonomy.Commands.DeactivateSkill;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Taxonomy;

public class DeactivateSkillCommandHandlerTests
{
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();
  private readonly ITaxonomyCache _cache = Substitute.For<ITaxonomyCache>();

  private DeactivateSkillCommandHandler CreateHandler() => new(_skillRepo, _cache);

  [Fact]
  public async Task Returns_NotFound_when_skill_missing()
  {
    var id = Guid.NewGuid();
    _skillRepo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Skill?)null);

    var result = await CreateHandler().Handle(new DeactivateSkillCommand(id), default);

    result.Should().Be(DeactivateSkillResult.NotFound);
    await _skillRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Deactivates_skill_and_invalidates_cache()
  {
    var skill = Skill.Create(Guid.NewGuid(), "Python", "Programming");
    _skillRepo.GetByIdAsync(skill.Id, Arg.Any<CancellationToken>()).Returns(skill);

    var result = await CreateHandler().Handle(new DeactivateSkillCommand(skill.Id), default);

    result.Should().Be(DeactivateSkillResult.Ok);
    skill.IsActive.Should().BeFalse();
    await _skillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    _cache.Received(1).Invalidate();
  }
}

public class ActivateSkillCommandHandlerTests
{
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();
  private readonly ITaxonomyCache _cache = Substitute.For<ITaxonomyCache>();

  private ActivateSkillCommandHandler CreateHandler() => new(_skillRepo, _cache);

  [Fact]
  public async Task Returns_NotFound_when_skill_missing()
  {
    var id = Guid.NewGuid();
    _skillRepo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Skill?)null);

    var result = await CreateHandler().Handle(new ActivateSkillCommand(id), default);

    result.Should().Be(ActivateSkillResult.NotFound);
    await _skillRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Activates_skill_and_invalidates_cache()
  {
    var skill = Skill.Create(Guid.NewGuid(), "Python", "Programming");
    skill.Deactivate();
    _skillRepo.GetByIdAsync(skill.Id, Arg.Any<CancellationToken>()).Returns(skill);

    var result = await CreateHandler().Handle(new ActivateSkillCommand(skill.Id), default);

    result.Should().Be(ActivateSkillResult.Ok);
    skill.IsActive.Should().BeTrue();
    await _skillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    _cache.Received(1).Invalidate();
  }
}
