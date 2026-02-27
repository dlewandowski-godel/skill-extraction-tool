using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Commands.DeactivateSkill;

public enum DeactivateSkillResult { Ok, NotFound }

public record DeactivateSkillCommand(Guid Id) : IRequest<DeactivateSkillResult>;

public class DeactivateSkillCommandHandler : IRequestHandler<DeactivateSkillCommand, DeactivateSkillResult>
{
  private readonly ISkillRepository _skillRepo;
  private readonly ITaxonomyCache _taxonomyCache;

  public DeactivateSkillCommandHandler(ISkillRepository skillRepo, ITaxonomyCache taxonomyCache)
  {
    _skillRepo = skillRepo;
    _taxonomyCache = taxonomyCache;
  }

  public async Task<DeactivateSkillResult> Handle(
      DeactivateSkillCommand request, CancellationToken cancellationToken)
  {
    var skill = await _skillRepo.GetByIdAsync(request.Id, cancellationToken);
    if (skill is null) return DeactivateSkillResult.NotFound;

    skill.Deactivate();
    await _skillRepo.SaveChangesAsync(cancellationToken);

    _taxonomyCache.Invalidate();
    return DeactivateSkillResult.Ok;
  }
}
