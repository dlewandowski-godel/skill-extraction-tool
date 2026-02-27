using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Application.Extraction;

/// <summary>
/// Transient DTO representing a skill extracted from a document during processing.
/// Not persisted directly — maps to EmployeeSkill after proficiency inference.
/// </summary>
public record ExtractedSkill(
    Guid SkillId,
    string SkillName,
    string Category,
    string MatchedAlias,
    int OccurrenceCount,
    ProficiencyLevel ProficiencyLevel = ProficiencyLevel.Intermediate,
    bool IsManualOverride = false);
