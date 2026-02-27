using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Queries.GetAllSkills;

public record TaxonomySkillDto(Guid SkillId, string Name, string Category);

public record GetAllSkillsQuery : IRequest<List<TaxonomySkillDto>>;

public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, List<TaxonomySkillDto>>
{
  private readonly ISkillRepository _skillRepo;

  public GetAllSkillsQueryHandler(ISkillRepository skillRepo)
      => _skillRepo = skillRepo;

  public async Task<List<TaxonomySkillDto>> Handle(GetAllSkillsQuery request, CancellationToken ct)
  {
    var skills = await _skillRepo.GetAllActiveAsync(ct);
    return skills
        .OrderBy(s => s.Category)
        .ThenBy(s => s.Name)
        .Select(s => new TaxonomySkillDto(s.Id, s.Name, s.Category))
        .ToList();
  }
}
