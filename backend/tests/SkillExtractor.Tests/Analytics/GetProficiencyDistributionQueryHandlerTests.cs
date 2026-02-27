using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Analytics.Queries.GetProficiencyDistribution;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.Analytics;

public class GetProficiencyDistributionQueryHandlerTests
{
  private readonly IAnalyticsRepository _repo = Substitute.For<IAnalyticsRepository>();
  private GetProficiencyDistributionQueryHandler CreateHandler() => new(_repo);

  [Fact]
  public async Task Returns_one_entry_per_proficiency_level_present()
  {
    var data = new List<ProficiencyDistributionDto>
        {
            new("Beginner", 5),
            new("Intermediate", 12),
            new("Advanced", 8),
            new("Expert", 3),
        };
    _repo.GetProficiencyDistributionAsync(Arg.Any<CancellationToken>()).Returns(data);

    var result = await CreateHandler().Handle(new GetProficiencyDistributionQuery(), default);

    result.Should().HaveCount(4);
    result.Select(r => r.Level).Should().BeEquivalentTo(new[] { "Beginner", "Intermediate", "Advanced", "Expert" });
  }

  [Fact]
  public async Task Counts_are_accurate()
  {
    _repo.GetProficiencyDistributionAsync(Arg.Any<CancellationToken>())
        .Returns(new List<ProficiencyDistributionDto> { new("Expert", 7) });

    var result = await CreateHandler().Handle(new GetProficiencyDistributionQuery(), default);

    result[0].Count.Should().Be(7);
  }

  [Fact]
  public async Task Returns_empty_list_when_no_skills_exist()
  {
    _repo.GetProficiencyDistributionAsync(Arg.Any<CancellationToken>()).Returns(new List<ProficiencyDistributionDto>());

    var result = await CreateHandler().Handle(new GetProficiencyDistributionQuery(), default);

    result.Should().BeEmpty();
  }
}
