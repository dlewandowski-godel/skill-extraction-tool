using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Commands.UpdateSkill;

public enum UpdateSkillResult { Ok, NotFound, Conflict }

public record UpdateSkillCommand(
    Guid Id,
    string Name,
    string Category,
    List<string> Aliases) : IRequest<UpdateSkillResult>;

public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, UpdateSkillResult>
{
  private readonly ISkillRepository _skillRepo;
  private readonly ITaxonomyCache _taxonomyCache;

  public UpdateSkillCommandHandler(ISkillRepository skillRepo, ITaxonomyCache taxonomyCache)
  {
    _skillRepo = skillRepo;
    _taxonomyCache = taxonomyCache;
  }

  public async Task<UpdateSkillResult> Handle(
      UpdateSkillCommand request, CancellationToken cancellationToken)
  {
    var skill = await _skillRepo.GetByIdAsync(request.Id, cancellationToken);
    if (skill is null) return UpdateSkillResult.NotFound;

    if (await _skillRepo.ExistsByNameAndCategoryAsync(request.Name, request.Category, request.Id, cancellationToken))
      return UpdateSkillResult.Conflict;

    // Ensure the name is always an alias
    var aliases = request.Aliases.ToList();
    if (!aliases.Any(a => string.Equals(a, request.Name, StringComparison.OrdinalIgnoreCase)))
      aliases.Insert(0, request.Name);

    skill.Update(request.Name, request.Category, aliases);
    await _skillRepo.SaveChangesAsync(cancellationToken);

    _taxonomyCache.Invalidate();
    return UpdateSkillResult.Ok;
  }
}
