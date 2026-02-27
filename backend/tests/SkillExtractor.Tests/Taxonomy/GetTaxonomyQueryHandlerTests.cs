using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Taxonomy.Queries.GetTaxonomy;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Taxonomy;

public class GetTaxonomyQueryHandlerTests
{
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();

  private GetTaxonomyQueryHandler CreateHandler() => new(_skillRepo);

  [Fact]
  public async Task Returns_empty_list_when_no_skills_exist()
  {
    _skillRepo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
        .Returns(new List<Skill>());

    var result = await CreateHandler().Handle(new GetTaxonomyQuery(null, null), default);

    result.Should().BeEmpty();
  }

  [Fact]
  public async Task Returns_all_skills_including_inactive()
  {
    var active = Skill.Create(Guid.NewGuid(), "Python", "Programming");
    var inactive = Skill.Create(Guid.NewGuid(), "COBOL", "Programming");
    inactive.Deactivate();

    _skillRepo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
        .Returns(new List<Skill> { active, inactive });

    var result = await CreateHandler().Handle(new GetTaxonomyQuery(null, null), default);

    result.Should().HaveCount(2);
    result.First(s => s.Name == "COBOL").IsActive.Should().BeFalse();
    result.First(s => s.Name == "Python").IsActive.Should().BeTrue();
  }

  [Fact]
  public async Task Maps_all_fields_including_aliases_and_created_at()
  {
    var id = Guid.NewGuid();
    var skill = Skill.Create(id, "React", "Frontend", new[] { "React", "ReactJS", "React.js" });

    _skillRepo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
        .Returns(new List<Skill> { skill });

    var result = await CreateHandler().Handle(new GetTaxonomyQuery(null, null), default);

    var dto = result.Single();
    dto.Id.Should().Be(id);
    dto.Name.Should().Be("React");
    dto.Category.Should().Be("Frontend");
    dto.Aliases.Should().BeEquivalentTo(new[] { "React", "ReactJS", "React.js" });
    dto.IsActive.Should().BeTrue();
    dto.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
  }

  [Fact]
  public async Task Passes_search_and_category_params_to_repository()
  {
    _skillRepo.GetAllAsync("py", "programming", Arg.Any<CancellationToken>())
        .Returns(new List<Skill>());

    await CreateHandler().Handle(new GetTaxonomyQuery("py", "programming"), default);

    await _skillRepo.Received(1).GetAllAsync("py", "programming", Arg.Any<CancellationToken>());
  }
}
