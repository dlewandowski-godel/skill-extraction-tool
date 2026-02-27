using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Queries.GetTaxonomy;

public record TaxonomyAdminSkillDto(
    Guid Id,
    string Name,
    string Category,
    List<string> Aliases,
    bool IsActive,
    DateTime CreatedAt);

public record GetTaxonomyQuery(string? Search, string? Category) : IRequest<List<TaxonomyAdminSkillDto>>;

public class GetTaxonomyQueryHandler : IRequestHandler<GetTaxonomyQuery, List<TaxonomyAdminSkillDto>>
{
  private readonly ISkillRepository _skillRepo;

  public GetTaxonomyQueryHandler(ISkillRepository skillRepo) => _skillRepo = skillRepo;

  public async Task<List<TaxonomyAdminSkillDto>> Handle(
      GetTaxonomyQuery request, CancellationToken cancellationToken)
  {
    var skills = await _skillRepo.GetAllAsync(request.Search, request.Category, cancellationToken);

    return skills
        .Select(s => new TaxonomyAdminSkillDto(
            s.Id, s.Name, s.Category, s.Aliases, s.IsActive, s.CreatedAt))
        .ToList();
  }
}
