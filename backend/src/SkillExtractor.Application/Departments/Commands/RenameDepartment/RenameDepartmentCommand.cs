using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Departments.Commands.RenameDepartment;

public record RenameDepartmentCommand(Guid DepartmentId, string NewName) : IRequest<RenameDepartmentResult>;

public enum RenameDepartmentResult { Ok, NotFound, DuplicateName }

public class RenameDepartmentCommandHandler
    : IRequestHandler<RenameDepartmentCommand, RenameDepartmentResult>
{
  private readonly IDepartmentRepository _repo;

  public RenameDepartmentCommandHandler(IDepartmentRepository repo) => _repo = repo;

  public async Task<RenameDepartmentResult> Handle(
      RenameDepartmentCommand request, CancellationToken cancellationToken)
  {
    var dept = await _repo.GetByIdAsync(request.DepartmentId, cancellationToken);
    if (dept is null) return RenameDepartmentResult.NotFound;

    if (await _repo.ExistsByNameAsync(request.NewName, cancellationToken))
      return RenameDepartmentResult.DuplicateName;

    dept.Rename(request.NewName);
    await _repo.SaveChangesAsync(cancellationToken);

    return RenameDepartmentResult.Ok;
  }
}
