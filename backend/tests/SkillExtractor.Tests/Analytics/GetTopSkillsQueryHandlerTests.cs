using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Analytics.Queries.GetTopSkills;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.Analytics;

public class GetTopSkillsQueryHandlerTests
{
    private readonly IAnalyticsRepository _repo = Substitute.For<IAnalyticsRepository>();
    private GetTopSkillsQueryHandler CreateHandler() => new(_repo);

    [Fact]
    public async Task Returns_sorted_descending_by_employee_count()
    {
        var data = new List<TopSkillDto>
        {
            new("Python", 8),
            new("React", 12),
            new("SQL", 5),
        };
        _repo.GetTopSkillsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(data);

        var result = await CreateHandler().Handle(new GetTopSkillsQuery(10), default);

        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Respects_limit_parameter()
    {
        _repo.GetTopSkillsAsync(3, Arg.Any<CancellationToken>()).Returns(new List<TopSkillDto>
        {
            new("A", 10), new("B", 8), new("C", 6),
        });

        await CreateHandler().Handle(new GetTopSkillsQuery(3), default);

        await _repo.Received(1).GetTopSkillsAsync(3, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Returns_empty_list_when_no_skills()
    {
        _repo.GetTopSkillsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new List<TopSkillDto>());

        var result = await CreateHandler().Handle(new GetTopSkillsQuery(10), default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Normalises_invalid_limit_to_10()
    {
        _repo.GetTopSkillsAsync(10, Arg.Any<CancellationToken>()).Returns(new List<TopSkillDto>());

        await CreateHandler().Handle(new GetTopSkillsQuery(0), default);

        await _repo.Received(1).GetTopSkillsAsync(10, Arg.Any<CancellationToken>());
    }
}
