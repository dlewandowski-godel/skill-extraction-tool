namespace SkillExtractor.Application.Profile;

public record SkillDto(
    Guid SkillId,
    string SkillName,
    string Category,
    string ProficiencyLevel,
    bool IsManualOverride,
    DateTime ExtractedAt);

public record EmployeeProfileDto(
    Guid UserId,
    string FullName,
    string? Department,
    List<SkillDto> Skills);
