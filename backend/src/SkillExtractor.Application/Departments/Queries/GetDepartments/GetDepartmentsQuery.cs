using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery : IRequest<List<DepartmentSummaryDto>>;

public class GetDepartmentsQueryHandler
    : IRequestHandler<GetDepartmentsQuery, List<DepartmentSummaryDto>>
{
  private readonly IDepartmentRepository _repo;

  public GetDepartmentsQueryHandler(IDepartmentRepository repo) => _repo = repo;

  public Task<List<DepartmentSummaryDto>> Handle(
      GetDepartmentsQuery request, CancellationToken cancellationToken)
      => _repo.GetAllAsync(cancellationToken);
}
