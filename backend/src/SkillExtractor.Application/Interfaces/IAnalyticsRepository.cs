using SkillExtractor.Application.Analytics;

namespace SkillExtractor.Application.Interfaces;

public interface IAnalyticsRepository
{
  /// <summary>US-6.1 — top N skills by employee count, sorted descending.</summary>
  Task<List<TopSkillDto>> GetTopSkillsAsync(int limit, CancellationToken ct = default);

  /// <summary>US-6.2 — skills grouped by department (users without a department are excluded).</summary>
  Task<List<DepartmentSkillsDto>> GetSkillsByDepartmentAsync(CancellationToken ct = default);

  /// <summary>
  /// US-6.3 — for each required skill in the given department (or all departments when null),
  /// returns the number of employees who have it and the total employee count in that department.
  /// Returns empty when no required skills are configured.
  /// </summary>
  Task<List<SkillGapRawDto>> GetRequiredSkillsWithCoverageAsync(string? department, CancellationToken ct = default);

  /// <summary>
  /// US-6.4 — raw (non-zero-filled) upload counts grouped by date and document type.
  /// The query handler performs zero-filling.
  /// </summary>
  Task<List<RawUploadPointDto>> GetUploadCountsByDateAsync(DateTime from, CancellationToken ct = default);

  /// <summary>US-6.5 — count of employee skill entries per proficiency level.</summary>
  Task<List<ProficiencyDistributionDto>> GetProficiencyDistributionAsync(CancellationToken ct = default);
}
