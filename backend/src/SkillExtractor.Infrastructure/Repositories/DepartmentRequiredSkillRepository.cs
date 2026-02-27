using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class DepartmentRequiredSkillRepository : IDepartmentRequiredSkillRepository
{
  private readonly AppDbContext _db;

  public DepartmentRequiredSkillRepository(AppDbContext db) => _db = db;

  public Task<List<DepartmentRequiredSkill>> GetByDepartmentNameAsync(
      string departmentName, CancellationToken ct) =>
      _db.DepartmentRequiredSkills
          .Include(d => d.Skill)
          .Where(d => d.DepartmentName.ToLower() == departmentName.ToLower())
          .ToListAsync(ct);

  public Task<bool> ExistsAsync(string departmentName, Guid skillId, CancellationToken ct) =>
      _db.DepartmentRequiredSkills.AnyAsync(
          d => d.DepartmentName.ToLower() == departmentName.ToLower() && d.SkillId == skillId, ct);

  public async Task AddAsync(DepartmentRequiredSkill entity, CancellationToken ct) =>
      await _db.DepartmentRequiredSkills.AddAsync(entity, ct);

  public Task<DepartmentRequiredSkill?> FindAsync(string departmentName, Guid skillId, CancellationToken ct) =>
      _db.DepartmentRequiredSkills.FirstOrDefaultAsync(
          d => d.DepartmentName.ToLower() == departmentName.ToLower() && d.SkillId == skillId, ct);

  public void Remove(DepartmentRequiredSkill entity) =>
      _db.DepartmentRequiredSkills.Remove(entity);

  public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
