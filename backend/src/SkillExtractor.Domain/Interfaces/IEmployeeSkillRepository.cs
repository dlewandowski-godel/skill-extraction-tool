using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Domain.Interfaces;

/// <summary>
/// Repository for the <see cref="EmployeeSkill"/> aggregate.
/// Defined in Domain per US-4.5.
/// </summary>
public interface IEmployeeSkillRepository
{
  Task<List<EmployeeSkill>> GetByUserAsync(Guid userId, CancellationToken ct = default);

  /// <summary>
  /// Removes all auto-extracted (non-manual) skills for <paramref name="userId"/>
  /// that originated from a document of type <paramref name="documentType"/>.
  /// Manual overrides are preserved.
  /// </summary>
  Task DeleteAutoExtractedByDocumentTypeAsync(
      Guid userId,
      DocumentType documentType,
      CancellationToken ct = default);

  void AddRange(IEnumerable<EmployeeSkill> skills);

  Task SaveChangesAsync(CancellationToken ct = default);
}
