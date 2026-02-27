using MediatR;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(string Name) : IRequest<CreateDepartmentResult>;

public record CreateDepartmentResult(bool Succeeded, Guid? DepartmentId, string? Error);

public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, CreateDepartmentResult>
{
  private readonly IDepartmentRepository _repo;

  public CreateDepartmentCommandHandler(IDepartmentRepository repo) => _repo = repo;

  public async Task<CreateDepartmentResult> Handle(
      CreateDepartmentCommand request, CancellationToken cancellationToken)
  {
    if (await _repo.ExistsByNameAsync(request.Name, cancellationToken))
      return new CreateDepartmentResult(false, null, "Department name already exists.");

    var dept = Department.Create(request.Name);
    await _repo.AddAsync(dept, cancellationToken);
    await _repo.SaveChangesAsync(cancellationToken);

    return new CreateDepartmentResult(true, dept.Id, null);
  }
}
