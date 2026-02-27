using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Analytics.Queries.GetSkillGaps;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.Analytics;

public class GetSkillGapsQueryHandlerTests
{
    private readonly IAnalyticsRepository _repo = Substitute.For<IAnalyticsRepository>();
    private GetSkillGapsQueryHandler CreateHandler() => new(_repo);

    [Fact]
    public async Task Returns_correct_gap_percent_when_some_employees_have_skill()
    {
        // 6 out of 10 employees have the skill → gap = 40 %
        _repo.GetRequiredSkillsWithCoverageAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new List<SkillGapRawDto> { new("Python", 6, 10) });

        var result = await CreateHandler().Handle(new GetSkillGapsQuery(), default);

        result.Should().HaveCount(1);
        result[0].GapPercent.Should().Be(40.0);
        result[0].EmployeesWithSkill.Should().Be(6);
        result[0].TotalEmployees.Should().Be(10);
    }

    [Fact]
    public async Task Returns_gap_percent_100_when_no_employees_have_skill()
    {
        _repo.GetRequiredSkillsWithCoverageAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new List<SkillGapRawDto> { new("Python", 0, 5) });

        var result = await CreateHandler().Handle(new GetSkillGapsQuery(), default);

        result[0].GapPercent.Should().Be(100.0);
    }

    [Fact]
    public async Task Returns_gap_percent_0_when_all_employees_have_skill()
    {
        _repo.GetRequiredSkillsWithCoverageAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new List<SkillGapRawDto> { new("Python", 5, 5) });

        var result = await CreateHandler().Handle(new GetSkillGapsQuery(), default);

        result[0].GapPercent.Should().Be(0.0);
    }

    [Fact]
    public async Task Returns_gap_percent_100_when_department_has_no_employees()
    {
        _repo.GetRequiredSkillsWithCoverageAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new List<SkillGapRawDto> { new("Python", 0, 0) });

        var result = await CreateHandler().Handle(new GetSkillGapsQuery(), default);

        result[0].GapPercent.Should().Be(100.0);
    }

    [Fact]
    public async Task Returns_empty_list_when_no_required_skills_configured()
    {
        _repo.GetRequiredSkillsWithCoverageAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new List<SkillGapRawDto>());

        var result = await CreateHandler().Handle(new GetSkillGapsQuery("Engineering"), default);

        result.Should().BeEmpty();
    }
}
