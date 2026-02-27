using Microsoft.EntityFrameworkCore;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class EmployeeSkillRepository : IEmployeeSkillRepository
{
  private readonly AppDbContext _db;

  public EmployeeSkillRepository(AppDbContext db) => _db = db;

  public Task<List<EmployeeSkill>> GetByUserAsync(Guid userId, CancellationToken ct = default)
      => _db.EmployeeSkills
            .Where(e => e.UserId == userId)
            .ToListAsync(ct);

  public Task<List<EmployeeSkill>> GetWithSkillsByUserAsync(Guid userId, CancellationToken ct = default)
      => _db.EmployeeSkills
            .Include(e => e.TaxonomySkill)
            .Where(e => e.UserId == userId)
            .ToListAsync(ct);

  public Task<EmployeeSkill?> GetByUserAndSkillAsync(Guid userId, Guid skillId, CancellationToken ct = default)
      => _db.EmployeeSkills
            .FirstOrDefaultAsync(e => e.UserId == userId && e.SkillId == skillId, ct);

  /// <summary>
  /// Removes all auto-extracted (non-manual) skills originating from documents of
  /// <paramref name="documentType"/> for the given user.
  /// Manual overrides are never touched.
  /// </summary>
  public Task DeleteAutoExtractedByDocumentTypeAsync(
      Guid userId,
      DocumentType documentType,
      CancellationToken ct = default)
      => _db.EmployeeSkills
            .Where(e => e.UserId == userId
                     && e.SourceDocumentType == documentType
                     && !e.IsManualOverride)
            .ExecuteDeleteAsync(ct);

  public void AddRange(IEnumerable<EmployeeSkill> skills)
      => _db.EmployeeSkills.AddRange(skills);

  public async Task AddAsync(EmployeeSkill skill, CancellationToken ct = default)
      => await _db.EmployeeSkills.AddAsync(skill, ct);

  public void Remove(EmployeeSkill skill)
      => _db.EmployeeSkills.Remove(skill);

  public Task SaveChangesAsync(CancellationToken ct = default)
      => _db.SaveChangesAsync(ct);
}
