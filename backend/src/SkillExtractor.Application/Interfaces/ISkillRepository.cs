using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Interfaces;

public interface ISkillRepository
{
  /// <summary>Returns only active skills (used by extraction engine).</summary>
  Task<List<Skill>> GetAllActiveAsync(CancellationToken ct = default);

  /// <summary>Returns all skills including inactive (used by admin taxonomy view).</summary>
  Task<List<Skill>> GetAllAsync(string? search = null, string? category = null, CancellationToken ct = default);

  Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default);
  Task AddAsync(Skill skill, CancellationToken ct = default);
  Task<bool> ExistsByNameAndCategoryAsync(string name, string category, Guid? excludeId = null, CancellationToken ct = default);
  Task SaveChangesAsync(CancellationToken ct = default);
}
