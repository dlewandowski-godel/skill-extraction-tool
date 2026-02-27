using MediatR;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Analytics.Queries.GetProficiencyDistribution;

public record GetProficiencyDistributionQuery : IRequest<List<ProficiencyDistributionDto>>;

public class GetProficiencyDistributionQueryHandler : IRequestHandler<GetProficiencyDistributionQuery, List<ProficiencyDistributionDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetProficiencyDistributionQueryHandler(IAnalyticsRepository repo) => _repo = repo;

  public Task<List<ProficiencyDistributionDto>> Handle(GetProficiencyDistributionQuery request, CancellationToken ct)
      => _repo.GetProficiencyDistributionAsync(ct);
}
