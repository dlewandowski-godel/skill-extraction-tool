using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Taxonomy.Queries.GetAllSkills;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Taxonomy;

public class GetAllSkillsQueryHandlerTests
{
  private readonly ISkillRepository _skillRepoSub = Substitute.For<ISkillRepository>();

  private GetAllSkillsQueryHandler CreateHandler()
      => new(_skillRepoSub);

  [Fact]
  public async Task Returns_empty_list_when_no_active_skills()
  {
    _skillRepoSub
        .GetAllActiveAsync(Arg.Any<CancellationToken>())
        .Returns(new List<Skill>());

    var result = await CreateHandler().Handle(new GetAllSkillsQuery(), default);

    Assert.Empty(result);
  }

  [Fact]
  public async Task Returns_skills_ordered_by_category_then_name()
  {
    var skills = new List<Skill>
        {
            Skill.Create(Guid.NewGuid(), "Python", "Programming"),
            Skill.Create(Guid.NewGuid(), "AutoCAD", "Design"),
            Skill.Create(Guid.NewGuid(), "C#", "Programming"),
        };
    _skillRepoSub
        .GetAllActiveAsync(Arg.Any<CancellationToken>())
        .Returns(skills);

    var result = await CreateHandler().Handle(new GetAllSkillsQuery(), default);

    Assert.Equal(3, result.Count);
    // Should be: AutoCAD (Design), C# (Programming), Python (Programming)
    Assert.Equal("AutoCAD", result[0].Name);
    Assert.Equal("C#", result[1].Name);
    Assert.Equal("Python", result[2].Name);
  }

  [Fact]
  public async Task Maps_skill_fields_correctly()
  {
    var id = Guid.NewGuid();
    _skillRepoSub
        .GetAllActiveAsync(Arg.Any<CancellationToken>())
        .Returns(new List<Skill> { Skill.Create(id, "React", "Frontend") });

    var result = await CreateHandler().Handle(new GetAllSkillsQuery(), default);

    Assert.Single(result);
    Assert.Equal(id, result[0].SkillId);
    Assert.Equal("React", result[0].Name);
    Assert.Equal("Frontend", result[0].Category);
  }
}
