using MediatR;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Taxonomy.Commands.AddRequiredSkill;

public enum AddRequiredSkillResult { Ok, DepartmentNotFound, SkillNotFound, AlreadyExists }

public record AddRequiredSkillCommand(Guid DepartmentId, Guid SkillId) : IRequest<AddRequiredSkillResult>;

public class AddRequiredSkillCommandHandler
    : IRequestHandler<AddRequiredSkillCommand, AddRequiredSkillResult>
{
  private readonly IDepartmentRepository _deptRepo;
  private readonly ISkillRepository _skillRepo;
  private readonly IDepartmentRequiredSkillRepository _requiredSkillRepo;

  public AddRequiredSkillCommandHandler(
      IDepartmentRepository deptRepo,
      ISkillRepository skillRepo,
      IDepartmentRequiredSkillRepository requiredSkillRepo)
  {
    _deptRepo = deptRepo;
    _skillRepo = skillRepo;
    _requiredSkillRepo = requiredSkillRepo;
  }

  public async Task<AddRequiredSkillResult> Handle(
      AddRequiredSkillCommand request, CancellationToken cancellationToken)
  {
    var dept = await _deptRepo.GetByIdAsync(request.DepartmentId, cancellationToken);
    if (dept is null) return AddRequiredSkillResult.DepartmentNotFound;

    var skill = await _skillRepo.GetByIdAsync(request.SkillId, cancellationToken);
    if (skill is null) return AddRequiredSkillResult.SkillNotFound;

    if (await _requiredSkillRepo.ExistsAsync(dept.Name, request.SkillId, cancellationToken))
      return AddRequiredSkillResult.AlreadyExists;

    var entity = DepartmentRequiredSkill.Create(dept.Name, request.SkillId);
    await _requiredSkillRepo.AddAsync(entity, cancellationToken);
    await _requiredSkillRepo.SaveChangesAsync(cancellationToken);

    return AddRequiredSkillResult.Ok;
  }
}
