using MediatR;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Taxonomy.Commands.AddSkill;

public enum AddSkillResult { Ok, Conflict }

public record AddSkillCommand(
    string Name,
    string Category,
    List<string> Aliases) : IRequest<(AddSkillResult Result, Guid? SkillId)>;

public class AddSkillCommandHandler
    : IRequestHandler<AddSkillCommand, (AddSkillResult Result, Guid? SkillId)>
{
  private readonly ISkillRepository _skillRepo;
  private readonly ITaxonomyCache _taxonomyCache;

  public AddSkillCommandHandler(ISkillRepository skillRepo, ITaxonomyCache taxonomyCache)
  {
    _skillRepo = skillRepo;
    _taxonomyCache = taxonomyCache;
  }

  public async Task<(AddSkillResult Result, Guid? SkillId)> Handle(
      AddSkillCommand request, CancellationToken cancellationToken)
  {
    if (await _skillRepo.ExistsByNameAndCategoryAsync(request.Name, request.Category, null, cancellationToken))
      return (AddSkillResult.Conflict, null);

    // Ensure the skill name itself is among its aliases
    var aliases = request.Aliases.ToList();
    if (!aliases.Any(a => string.Equals(a, request.Name, StringComparison.OrdinalIgnoreCase)))
      aliases.Insert(0, request.Name);

    var skill = Skill.Create(Guid.NewGuid(), request.Name, request.Category, aliases);
    await _skillRepo.AddAsync(skill, cancellationToken);
    await _skillRepo.SaveChangesAsync(cancellationToken);

    _taxonomyCache.Invalidate();

    return (AddSkillResult.Ok, skill.Id);
  }
}
