using MediatR;
using SkillExtractor.Application.Common;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.EmployeeManagement.Queries.GetEmployees;

public record GetEmployeesQuery(
    string? Search,
    string? Department,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<EmployeeListItemDto>>;

public class GetEmployeesQueryHandler
    : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeListItemDto>>
{
  private readonly IEmployeeRepository _repo;

  public GetEmployeesQueryHandler(IEmployeeRepository repo) => _repo = repo;

  public async Task<PagedResult<EmployeeListItemDto>> Handle(
      GetEmployeesQuery request, CancellationToken cancellationToken)
  {
    var page = request.Page < 1 ? 1 : request.Page;
    var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

    return await _repo.GetPagedAsync(
        request.Search,
        request.Department,
        page,
        pageSize,
        cancellationToken);
  }
}
