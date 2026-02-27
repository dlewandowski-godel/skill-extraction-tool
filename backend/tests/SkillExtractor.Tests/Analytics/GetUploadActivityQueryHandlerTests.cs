using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Analytics.Queries.GetUploadActivity;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Tests.Analytics;

public class GetUploadActivityQueryHandlerTests
{
    private readonly IAnalyticsRepository _repo = Substitute.For<IAnalyticsRepository>();
    private GetUploadActivityQueryHandler CreateHandler() => new(_repo);

    [Fact]
    public async Task Returns_exactly_n_entries_for_the_requested_period()
    {
        _repo.GetUploadCountsByDateAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<RawUploadPointDto>());

        var result = await CreateHandler().Handle(new GetUploadActivityQuery(7), default);

        result.Should().HaveCount(7);
    }

    [Fact]
    public async Task Zero_fills_days_with_no_uploads()
    {
        // Raw data only has one day with a CV upload
        var today = DateTime.UtcNow.Date;
        var from = today.AddDays(-6); // 7-day period

        _repo.GetUploadCountsByDateAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<RawUploadPointDto>
            {
                new(from, DocumentType.CV, 3),
            });

        var result = await CreateHandler().Handle(new GetUploadActivityQuery(7), default);

        result.Should().HaveCount(7);
        // The day with data should have the correct count
        result[0].CvCount.Should().Be(3);
        result[0].IfuCount.Should().Be(0);
        // Days without data should be zero-filled
        result.Skip(1).Should().AllSatisfy(d => d.CvCount.Should().Be(0));
        result.Skip(1).Should().AllSatisfy(d => d.IfuCount.Should().Be(0));
    }

    [Fact]
    public async Task Reports_cv_and_ifu_counts_separately_per_day()
    {
        var today = DateTime.UtcNow.Date;
        var from = today.AddDays(-2); // 3-day period

        _repo.GetUploadCountsByDateAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<RawUploadPointDto>
            {
                new(from, DocumentType.CV, 4),
                new(from, DocumentType.IFU, 2),
            });

        var result = await CreateHandler().Handle(new GetUploadActivityQuery(3), default);

        result[0].CvCount.Should().Be(4);
        result[0].IfuCount.Should().Be(2);
    }

    [Fact]
    public async Task Period_30d_returns_exactly_30_entries()
    {
        _repo.GetUploadCountsByDateAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<RawUploadPointDto>());

        var result = await CreateHandler().Handle(new GetUploadActivityQuery(30), default);

        result.Should().HaveCount(30);
    }
}
