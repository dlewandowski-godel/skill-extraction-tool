using MediatR;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Analytics.Queries.GetTopSkills;

public record GetTopSkillsQuery(int Limit = 10) : IRequest<List<TopSkillDto>>;

public class GetTopSkillsQueryHandler : IRequestHandler<GetTopSkillsQuery, List<TopSkillDto>>
{
    private readonly IAnalyticsRepository _repo;

    public GetTopSkillsQueryHandler(IAnalyticsRepository repo) => _repo = repo;

    public Task<List<TopSkillDto>> Handle(GetTopSkillsQuery request, CancellationToken ct)
    {
        var limit = request.Limit is > 0 and <= 100 ? request.Limit : 10;
        return _repo.GetTopSkillsAsync(limit, ct);
    }
}
