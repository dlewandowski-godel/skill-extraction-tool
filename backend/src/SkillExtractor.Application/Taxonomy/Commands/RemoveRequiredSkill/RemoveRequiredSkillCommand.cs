using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Taxonomy.Commands.RemoveRequiredSkill;

public enum RemoveRequiredSkillResult { Ok, NotFound }

public record RemoveRequiredSkillCommand(Guid DepartmentId, Guid SkillId) : IRequest<RemoveRequiredSkillResult>;

public class RemoveRequiredSkillCommandHandler
    : IRequestHandler<RemoveRequiredSkillCommand, RemoveRequiredSkillResult>
{
  private readonly IDepartmentRepository _deptRepo;
  private readonly IDepartmentRequiredSkillRepository _requiredSkillRepo;

  public RemoveRequiredSkillCommandHandler(
      IDepartmentRepository deptRepo,
      IDepartmentRequiredSkillRepository requiredSkillRepo)
  {
    _deptRepo = deptRepo;
    _requiredSkillRepo = requiredSkillRepo;
  }

  public async Task<RemoveRequiredSkillResult> Handle(
      RemoveRequiredSkillCommand request, CancellationToken cancellationToken)
  {
    var dept = await _deptRepo.GetByIdAsync(request.DepartmentId, cancellationToken);
    if (dept is null) return RemoveRequiredSkillResult.NotFound;

    var entry = await _requiredSkillRepo.FindAsync(dept.Name, request.SkillId, cancellationToken);
    if (entry is null) return RemoveRequiredSkillResult.NotFound;

    _requiredSkillRepo.Remove(entry);
    await _requiredSkillRepo.SaveChangesAsync(cancellationToken);

    return RemoveRequiredSkillResult.Ok;
  }
}
