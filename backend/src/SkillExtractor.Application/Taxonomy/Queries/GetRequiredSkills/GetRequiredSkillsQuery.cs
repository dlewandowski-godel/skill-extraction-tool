using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Queries.GetRequiredSkills;

public record RequiredSkillDto(Guid SkillId, string Name, string Category);

public record GetRequiredSkillsQuery(Guid DepartmentId) : IRequest<List<RequiredSkillDto>?>;

public class GetRequiredSkillsQueryHandler
    : IRequestHandler<GetRequiredSkillsQuery, List<RequiredSkillDto>?>
{
  private readonly IDepartmentRepository _deptRepo;
  private readonly IDepartmentRequiredSkillRepository _requiredSkillRepo;

  public GetRequiredSkillsQueryHandler(
      IDepartmentRepository deptRepo,
      IDepartmentRequiredSkillRepository requiredSkillRepo)
  {
    _deptRepo = deptRepo;
    _requiredSkillRepo = requiredSkillRepo;
  }

  public async Task<List<RequiredSkillDto>?> Handle(
      GetRequiredSkillsQuery request, CancellationToken cancellationToken)
  {
    var dept = await _deptRepo.GetByIdAsync(request.DepartmentId, cancellationToken);
    if (dept is null) return null;

    var entries = await _requiredSkillRepo.GetByDepartmentNameAsync(dept.Name, cancellationToken);

    return entries
        .Where(e => e.Skill is not null)
        .Select(e => new RequiredSkillDto(e.SkillId, e.Skill!.Name, e.Skill.Category))
        .OrderBy(s => s.Category)
        .ThenBy(s => s.Name)
        .ToList();
  }
}
