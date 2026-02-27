using MediatR;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Analytics.Queries.GetSkillsByDepartment;

public record GetSkillsByDepartmentQuery : IRequest<List<DepartmentSkillsDto>>;

public class GetSkillsByDepartmentQueryHandler : IRequestHandler<GetSkillsByDepartmentQuery, List<DepartmentSkillsDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetSkillsByDepartmentQueryHandler(IAnalyticsRepository repo) => _repo = repo;

  public Task<List<DepartmentSkillsDto>> Handle(GetSkillsByDepartmentQuery request, CancellationToken ct)
      => _repo.GetSkillsByDepartmentAsync(ct);
}
