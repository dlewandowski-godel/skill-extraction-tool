using SkillExtractor.Application.Common;

namespace SkillExtractor.Application.Interfaces;

public record EmployeeListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? DepartmentName,
    string Role,
    bool IsActive,
    DateTime? LastUploadDate);

public record DepartmentSummaryDto(
    Guid Id,
    string Name,
    int EmployeeCount,
    int RequiredSkillCount);

public interface IEmployeeRepository
{
  Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(
      string? search,
      string? department,
      int page,
      int pageSize,
      CancellationToken ct = default);
}

public interface IDepartmentRepository
{
  Task<List<DepartmentSummaryDto>> GetAllAsync(CancellationToken ct = default);
  Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
  Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
  Task<bool> HasEmployeesAsync(Guid id, CancellationToken ct = default);
  Task AddAsync(Domain.Entities.Department department, CancellationToken ct = default);
  Task<Domain.Entities.Department?> GetByIdAsync(Guid id, CancellationToken ct = default);
  Task DeleteAsync(Guid id, CancellationToken ct = default);
  Task SaveChangesAsync(CancellationToken ct = default);
}
