using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Commands.ActivateSkill;

public enum ActivateSkillResult { Ok, NotFound }

public record ActivateSkillCommand(Guid Id) : IRequest<ActivateSkillResult>;

public class ActivateSkillCommandHandler : IRequestHandler<ActivateSkillCommand, ActivateSkillResult>
{
  private readonly ISkillRepository _skillRepo;
  private readonly ITaxonomyCache _taxonomyCache;

  public ActivateSkillCommandHandler(ISkillRepository skillRepo, ITaxonomyCache taxonomyCache)
  {
    _skillRepo = skillRepo;
    _taxonomyCache = taxonomyCache;
  }

  public async Task<ActivateSkillResult> Handle(
      ActivateSkillCommand request, CancellationToken cancellationToken)
  {
    var skill = await _skillRepo.GetByIdAsync(request.Id, cancellationToken);
    if (skill is null) return ActivateSkillResult.NotFound;

    skill.Activate();
    await _skillRepo.SaveChangesAsync(cancellationToken);

    _taxonomyCache.Invalidate();
    return ActivateSkillResult.Ok;
  }
}
