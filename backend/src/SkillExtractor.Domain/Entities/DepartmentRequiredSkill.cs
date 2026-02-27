namespace SkillExtractor.Domain.Entities;

/// <summary>
/// Represents a skill that is required for employees in a given department.
/// Managed via the taxonomy admin UI (US-8.5).
/// Used by the skill gap analysis (US-6.3).
/// </summary>
public class DepartmentRequiredSkill
{
    public Guid Id { get; private set; }
    public string DepartmentName { get; private set; } = string.Empty;
    public Guid SkillId { get; private set; }

    /// <summary>Navigation property — loaded via .Include().</summary>
    public Skill? Skill { get; private set; }

    private DepartmentRequiredSkill() { }

    public static DepartmentRequiredSkill Create(string departmentName, Guid skillId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(departmentName);
        if (skillId == Guid.Empty) throw new ArgumentException("SkillId must not be empty.", nameof(skillId));
        return new DepartmentRequiredSkill
        {
            Id = Guid.NewGuid(),
            DepartmentName = departmentName,
            SkillId = skillId,
        };
    }
}
