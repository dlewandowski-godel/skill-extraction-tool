namespace SkillExtractor.Application.Extraction;

/// <summary>
/// Flat entry from the skill taxonomy alias map — used for O(1) lookup during matching.
/// </summary>
public record SkillTaxonomyEntry(
    Guid SkillId,
    string SkillName,
    string Category,
    string Alias);
