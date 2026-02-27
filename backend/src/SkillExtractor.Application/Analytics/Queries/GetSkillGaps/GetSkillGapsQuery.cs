using MediatR;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Analytics.Queries.GetSkillGaps;

public record GetSkillGapsQuery(string? Department = null) : IRequest<List<SkillGapDto>>;

public class GetSkillGapsQueryHandler : IRequestHandler<GetSkillGapsQuery, List<SkillGapDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetSkillGapsQueryHandler(IAnalyticsRepository repo) => _repo = repo;

  public async Task<List<SkillGapDto>> Handle(GetSkillGapsQuery request, CancellationToken ct)
  {
    var raw = await _repo.GetRequiredSkillsWithCoverageAsync(request.Department, ct);

    return raw.Select(r =>
    {
      var gapPercent = r.TotalEmployeesInDept == 0
              ? 100.0
              : Math.Round(100.0 - (r.EmployeesWithSkill / (double)r.TotalEmployeesInDept * 100.0), 1);

      return new SkillGapDto(r.SkillName, r.EmployeesWithSkill, r.TotalEmployeesInDept, gapPercent);
    }).ToList();
  }
}
