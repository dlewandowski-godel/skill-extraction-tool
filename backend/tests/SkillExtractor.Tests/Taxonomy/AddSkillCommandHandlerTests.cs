using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Taxonomy.Commands.AddSkill;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Taxonomy;

public class AddSkillCommandHandlerTests
{
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();
  private readonly ITaxonomyCache _cache = Substitute.For<ITaxonomyCache>();

  private AddSkillCommandHandler CreateHandler() => new(_skillRepo, _cache);

  [Fact]
  public async Task Returns_Conflict_when_name_and_category_already_exist()
  {
    _skillRepo.ExistsByNameAndCategoryAsync("Python", "Programming", null, Arg.Any<CancellationToken>())
        .Returns(true);

    var (result, skillId) = await CreateHandler().Handle(
        new AddSkillCommand("Python", "Programming", new List<string> { "Python" }), default);

    result.Should().Be(AddSkillResult.Conflict);
    skillId.Should().BeNull();
    await _skillRepo.DidNotReceive().AddAsync(Arg.Any<Skill>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Creates_skill_and_invalidates_cache_on_success()
  {
    _skillRepo.ExistsByNameAndCategoryAsync(Arg.Any<string>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>())
        .Returns(false);

    var (result, skillId) = await CreateHandler().Handle(
        new AddSkillCommand("Python", "Programming", new List<string> { "Python", "py" }), default);

    result.Should().Be(AddSkillResult.Ok);
    skillId.Should().NotBeNull();
    await _skillRepo.Received(1).AddAsync(Arg.Any<Skill>(), Arg.Any<CancellationToken>());
    await _skillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    _cache.Received(1).Invalidate();
  }

  [Fact]
  public async Task Auto_adds_skill_name_to_aliases_if_missing()
  {
    _skillRepo.ExistsByNameAndCategoryAsync(Arg.Any<string>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>())
        .Returns(false);

    Skill? captured = null;
    await _skillRepo.AddAsync(
        Arg.Do<Skill>(s => captured = s),
        Arg.Any<CancellationToken>());

    await CreateHandler().Handle(
        new AddSkillCommand("Python", "Programming", new List<string> { "py" }), default);

    captured.Should().NotBeNull();
    captured!.Aliases.Should().Contain("Python");
  }

  [Fact]
  public async Task Does_not_duplicate_name_in_aliases_when_already_present()
  {
    _skillRepo.ExistsByNameAndCategoryAsync(Arg.Any<string>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>())
        .Returns(false);

    Skill? captured = null;
    await _skillRepo.AddAsync(
        Arg.Do<Skill>(s => captured = s),
        Arg.Any<CancellationToken>());

    await CreateHandler().Handle(
        new AddSkillCommand("Python", "Programming", new List<string> { "Python", "py" }), default);

    captured!.Aliases.Count(a => string.Equals(a, "Python", StringComparison.OrdinalIgnoreCase))
        .Should().Be(1);
  }
}
