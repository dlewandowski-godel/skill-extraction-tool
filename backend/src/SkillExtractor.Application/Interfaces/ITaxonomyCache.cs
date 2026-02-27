using SkillExtractor.Application.Extraction;

namespace SkillExtractor.Application.Interfaces;

/// <summary>
/// In-memory cache of the skill taxonomy, keyed by normalised alias string.
/// Invalidated when admin adds or updates a skill (Epic 8).
/// </summary>
public interface ITaxonomyCache
{
  /// <summary>Alias (lowercase) → taxonomy entry, ready for O(1) lookup.</summary>
  IReadOnlyDictionary<string, SkillTaxonomyEntry> AliasMap { get; }

  /// <summary>Marks the cache as stale so it is reloaded on next access.</summary>
  void Invalidate();
}
