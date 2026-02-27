using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Domain.Entities;

/// <summary>
/// Links an employee (User) to a skill from the taxonomy, along with inferred or
/// manually-set proficiency level.
/// </summary>
public class EmployeeSkill
{
  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public Guid SkillId { get; private set; }
  public ProficiencyLevel ProficiencyLevel { get; private set; }

  /// <summary>
  /// When true this record was set by an admin and must not be overwritten by re-extraction.
  /// </summary>
  public bool IsManualOverride { get; private set; }

  /// <summary>The document whose extraction produced this skill record.</summary>
  public Guid SourceDocumentId { get; private set; }

  /// <summary>Tracks whether the source was a CV or IFU so re-extraction can replace the right slice.</summary>
  public DocumentType SourceDocumentType { get; private set; }

  public DateTime ExtractedAt { get; private set; }

  // EF Core
  private EmployeeSkill() { }

  /// <summary>Creates an auto-extracted skill record.</summary>
  public static EmployeeSkill Create(
      Guid userId,
      Guid skillId,
      ProficiencyLevel proficiencyLevel,
      Guid sourceDocumentId,
      DocumentType sourceDocumentType)
  {
    if (userId == Guid.Empty) throw new ArgumentException("UserId must not be empty.", nameof(userId));
    if (skillId == Guid.Empty) throw new ArgumentException("SkillId must not be empty.", nameof(skillId));

    return new EmployeeSkill
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      SkillId = skillId,
      ProficiencyLevel = proficiencyLevel,
      IsManualOverride = false,
      SourceDocumentId = sourceDocumentId,
      SourceDocumentType = sourceDocumentType,
      ExtractedAt = DateTime.UtcNow,
    };
  }

  /// <summary>Admin override — sets proficiency and locks it against auto-extraction.</summary>
  public void SetManualOverrideProficiency(ProficiencyLevel level)
  {
    ProficiencyLevel = level;
    IsManualOverride = true;
  }

  /// <summary>
  /// Updates proficiency during auto-extraction merge (CV + IFU highest-wins).
  /// Does NOT change <see cref="IsManualOverride"/>.
  /// </summary>
  public void UpdateAutoExtractedProficiency(ProficiencyLevel level)
  {
    ProficiencyLevel = level;
  }
}
