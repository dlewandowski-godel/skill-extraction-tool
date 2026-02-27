using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Application.Analytics;

/// <summary>US-6.1 — a single skill with the count of employees who have it.</summary>
public record TopSkillDto(string SkillName, int EmployeeCount);

/// <summary>US-6.2 — a single skill entry within a department.</summary>
public record DepartmentSkillDto(string Name, int Count);

/// <summary>US-6.2 — all skills grouped under one department.</summary>
public record DepartmentSkillsDto(string Department, List<DepartmentSkillDto> Skills);

/// <summary>
/// US-6.3 — raw coverage data returned by the repository.
/// The handler computes <see cref="SkillGapDto.GapPercent"/> from this.
/// </summary>
public record SkillGapRawDto(string SkillName, int EmployeesWithSkill, int TotalEmployeesInDept);

/// <summary>US-6.3 — computed gap result returned to the API.</summary>
public record SkillGapDto(string SkillName, int EmployeesWithSkill, int TotalEmployees, double GapPercent);

/// <summary>
/// US-6.4 — raw upload data point from the repository (before zero-filling).
/// </summary>
public record RawUploadPointDto(DateTime Date, DocumentType DocumentType, int Count);

/// <summary>US-6.4 — one entry per day with CV and IFU counts (zero-filled).</summary>
public record UploadActivityDto(string Date, int CvCount, int IfuCount);

/// <summary>US-6.5 — count of employee skills at one proficiency level.</summary>
public record ProficiencyDistributionDto(string Level, int Count);
