using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Taxonomy.Commands.UpdateSkill;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Taxonomy;

public class UpdateSkillCommandHandlerTests
{
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();
  private readonly ITaxonomyCache _cache = Substitute.For<ITaxonomyCache>();

  private UpdateSkillCommandHandler CreateHandler() => new(_skillRepo, _cache);

  [Fact]
  public async Task Returns_NotFound_when_skill_does_not_exist()
  {
    var id = Guid.NewGuid();
    _skillRepo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Skill?)null);

    var result = await CreateHandler().Handle(
        new UpdateSkillCommand(id, "Python", "Programming", new List<string> { "Python" }), default);

    result.Should().Be(UpdateSkillResult.NotFound);
    await _skillRepo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Returns_Conflict_when_another_skill_has_same_name_and_category()
  {
    var id = Guid.NewGuid();
    var skill = Skill.Create(id, "Python", "Programming");
    _skillRepo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(skill);
    _skillRepo.ExistsByNameAndCategoryAsync("Python", "Programming", id, Arg.Any<CancellationToken>())
        .Returns(true);

    var result = await CreateHandler().Handle(
        new UpdateSkillCommand(id, "Python", "Programming", new List<string> { "Python" }), default);

    result.Should().Be(UpdateSkillResult.Conflict);
  }

  [Fact]
  public async Task Updates_skill_and_invalidates_cache_on_success()
  {
    var id = Guid.NewGuid();
    var skill = Skill.Create(id, "Python", "Programming");
    _skillRepo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(skill);
    _skillRepo.ExistsByNameAndCategoryAsync(Arg.Any<string>(), Arg.Any<string>(), id, Arg.Any<CancellationToken>())
        .Returns(false);

    var result = await CreateHandler().Handle(
        new UpdateSkillCommand(id, "Python 3", "Programming", new List<string> { "Python 3", "py3" }), default);

    result.Should().Be(UpdateSkillResult.Ok);
    skill.Name.Should().Be("Python 3");
    await _skillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    _cache.Received(1).Invalidate();
  }
}
