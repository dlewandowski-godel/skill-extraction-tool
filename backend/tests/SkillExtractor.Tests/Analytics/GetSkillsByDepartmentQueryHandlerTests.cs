using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Analytics.Queries.GetSkillsByDepartment;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.Analytics;

public class GetSkillsByDepartmentQueryHandlerTests
{
  private readonly IAnalyticsRepository _repo = Substitute.For<IAnalyticsRepository>();
  private GetSkillsByDepartmentQueryHandler CreateHandler() => new(_repo);

  [Fact]
  public async Task Returns_one_entry_per_department()
  {
    var data = new List<DepartmentSkillsDto>
        {
            new("Engineering", new() { new("Python", 5), new("C#", 3) }),
            new("Design", new() { new("Figma", 4) }),
        };
    _repo.GetSkillsByDepartmentAsync(Arg.Any<CancellationToken>()).Returns(data);

    var result = await CreateHandler().Handle(new GetSkillsByDepartmentQuery(), default);

    result.Should().HaveCount(2);
    result.Select(r => r.Department).Should().BeEquivalentTo(new[] { "Engineering", "Design" });
  }

  [Fact]
  public async Task Returns_empty_when_no_departments_with_skills()
  {
    _repo.GetSkillsByDepartmentAsync(Arg.Any<CancellationToken>()).Returns(new List<DepartmentSkillsDto>());

    var result = await CreateHandler().Handle(new GetSkillsByDepartmentQuery(), default);

    result.Should().BeEmpty();
  }

  [Fact]
  public async Task Calls_repository_once()
  {
    _repo.GetSkillsByDepartmentAsync(Arg.Any<CancellationToken>()).Returns(new List<DepartmentSkillsDto>());

    await CreateHandler().Handle(new GetSkillsByDepartmentQuery(), default);

    await _repo.Received(1).GetSkillsByDepartmentAsync(Arg.Any<CancellationToken>());
  }
}
