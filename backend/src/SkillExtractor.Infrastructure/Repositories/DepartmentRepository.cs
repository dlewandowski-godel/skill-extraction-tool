using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
  private readonly AppDbContext _db;

  public DepartmentRepository(AppDbContext db) => _db = db;

  public async Task<List<DepartmentSummaryDto>> GetAllAsync(CancellationToken ct = default)
  {
    var depts = await _db.Departments.OrderBy(d => d.Name).ToListAsync(ct);
    var employeeCounts = await _db.Users
        .Where(u => u.DepartmentId != null)
        .GroupBy(u => u.DepartmentId)
        .Select(g => new { DeptId = g.Key!.Value, Count = g.Count() })
        .ToListAsync(ct);

    return depts.Select(d =>
    {
      var ec = employeeCounts.FirstOrDefault(x => x.DeptId == d.Id)?.Count ?? 0;
      return new DepartmentSummaryDto(d.Id, d.Name, ec, 0); // RequiredSkillCount populated in US-8.5
    }).ToList();
  }

  public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
      await _db.Departments.AnyAsync(d => d.Name.ToLower() == name.ToLower().Trim(), ct);

  public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default) =>
      await _db.Departments.AnyAsync(d => d.Id == id, ct);

  public async Task<bool> HasEmployeesAsync(Guid id, CancellationToken ct = default) =>
      await _db.Users.AnyAsync(u => u.DepartmentId == id, ct);

  public async Task AddAsync(Department department, CancellationToken ct = default) =>
      await _db.Departments.AddAsync(department, ct);

  public async Task<Department?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
      await _db.Departments.FindAsync([id], ct);

  public async Task DeleteAsync(Guid id, CancellationToken ct = default)
  {
    var dept = await _db.Departments.FindAsync([id], ct);
    if (dept is not null) _db.Departments.Remove(dept);
  }

  public Task SaveChangesAsync(CancellationToken ct = default) =>
      _db.SaveChangesAsync(ct);
}
