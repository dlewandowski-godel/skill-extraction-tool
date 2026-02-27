using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Interfaces;

public interface IDepartmentRequiredSkillRepository
{
  Task<List<DepartmentRequiredSkill>> GetByDepartmentNameAsync(string departmentName, CancellationToken ct = default);
  Task<bool> ExistsAsync(string departmentName, Guid skillId, CancellationToken ct = default);
  Task AddAsync(DepartmentRequiredSkill entity, CancellationToken ct = default);
  Task<DepartmentRequiredSkill?> FindAsync(string departmentName, Guid skillId, CancellationToken ct = default);
  void Remove(DepartmentRequiredSkill entity);
  Task SaveChangesAsync(CancellationToken ct = default);
}
