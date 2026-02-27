using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid DepartmentId) : IRequest<DeleteDepartmentResult>;

public enum DeleteDepartmentResult { Ok, NotFound, HasEmployees }

public class DeleteDepartmentCommandHandler
    : IRequestHandler<DeleteDepartmentCommand, DeleteDepartmentResult>
{
  private readonly IDepartmentRepository _repo;

  public DeleteDepartmentCommandHandler(IDepartmentRepository repo) => _repo = repo;

  public async Task<DeleteDepartmentResult> Handle(
      DeleteDepartmentCommand request, CancellationToken cancellationToken)
  {
    if (!await _repo.ExistsByIdAsync(request.DepartmentId, cancellationToken))
      return DeleteDepartmentResult.NotFound;

    if (await _repo.HasEmployeesAsync(request.DepartmentId, cancellationToken))
      return DeleteDepartmentResult.HasEmployees;

    await _repo.DeleteAsync(request.DepartmentId, cancellationToken);
    await _repo.SaveChangesAsync(cancellationToken);

    return DeleteDepartmentResult.Ok;
  }
}
