using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
  private readonly AppDbContext _db;

  public SkillRepository(AppDbContext db) => _db = db;

  public Task<List<Skill>> GetAllActiveAsync(CancellationToken ct = default)
      => _db.Skills.Where(s => s.IsActive).ToListAsync(ct);

  public Task<List<Skill>> GetAllAsync(string? search = null, string? category = null, CancellationToken ct = default)
  {
    var query = _db.Skills.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
      query = query.Where(s => s.Name.ToLower().Contains(search.ToLower()));
    if (!string.IsNullOrWhiteSpace(category))
      query = query.Where(s => s.Category.ToLower() == category.ToLower());
    return query.OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync(ct);
  }

  public Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default)
      => _db.Skills.FirstOrDefaultAsync(s => s.Id == id, ct);

  public async Task AddAsync(Skill skill, CancellationToken ct = default)
      => await _db.Skills.AddAsync(skill, ct);

  public Task<bool> ExistsByNameAndCategoryAsync(string name, string category, Guid? excludeId = null, CancellationToken ct = default)
  {
    var q = _db.Skills.Where(s => s.Name.ToLower() == name.ToLower() && s.Category.ToLower() == category.ToLower());
    if (excludeId.HasValue) q = q.Where(s => s.Id != excludeId.Value);
    return q.AnyAsync(ct);
  }

  public Task SaveChangesAsync(CancellationToken ct = default)
      => _db.SaveChangesAsync(ct);
}
