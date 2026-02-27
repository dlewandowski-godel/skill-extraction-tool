using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
  private readonly AppDbContext _db;

  public AnalyticsRepository(AppDbContext db) => _db = db;

  // ── US-6.1 ────────────────────────────────────────────────────────────────

  public async Task<List<TopSkillDto>> GetTopSkillsAsync(int limit, CancellationToken ct = default)
  {
    var data = await (
        from es in _db.EmployeeSkills
        join s in _db.Skills on es.SkillId equals s.Id
        group es by new { s.Id, s.Name } into g
        select new { g.Key.Name, EmployeeCount = g.Select(x => x.UserId).Distinct().Count() }
    ).ToListAsync(ct);

    return data
        .OrderByDescending(x => x.EmployeeCount)
        .Take(limit)
        .Select(x => new TopSkillDto(x.Name, x.EmployeeCount))
        .ToList();
  }

  // ── US-6.2 ────────────────────────────────────────────────────────────────

  public async Task<List<DepartmentSkillsDto>> GetSkillsByDepartmentAsync(CancellationToken ct = default)
  {
    var data = await (
        from es in _db.EmployeeSkills
        join u in _db.Users on es.UserId equals u.Id
        join d in _db.Departments on u.DepartmentId equals d.Id
        join s in _db.Skills on es.SkillId equals s.Id
        select new { DeptName = d.Name, SkillName = s.Name }
    ).ToListAsync(ct);

    return data
        .GroupBy(x => x.DeptName)
        .Select(deptGrp => new DepartmentSkillsDto(
            deptGrp.Key,
            deptGrp
                .GroupBy(x => x.SkillName)
                .Select(skillGrp => new DepartmentSkillDto(skillGrp.Key, skillGrp.Count()))
                .OrderByDescending(x => x.Count)
                .ToList()))
        .OrderBy(d => d.Department)
        .ToList();
  }

  // ── US-6.3 ────────────────────────────────────────────────────────────────

  public async Task<List<SkillGapRawDto>> GetRequiredSkillsWithCoverageAsync(
      string? department, CancellationToken ct = default)
  {
    var required = await _db.DepartmentRequiredSkills
        .Where(drs => department == null || drs.DepartmentName == department)
        .Include(drs => drs.Skill)
        .ToListAsync(ct);

    if (required.Count == 0) return [];

    var deptNames = required.Select(r => r.DepartmentName).Distinct().ToList();
    var skillIds = required.Select(r => r.SkillId).Distinct().ToList();

    // Coverage: how many employees in each dept have each required skill
    var coverage = await (
        from es in _db.EmployeeSkills
        join u in _db.Users on es.UserId equals u.Id
        join d in _db.Departments on u.DepartmentId equals d.Id
        where skillIds.Contains(es.SkillId) && deptNames.Contains(d.Name)
        group es by new { es.SkillId, DeptName = d.Name } into g
        select new { g.Key.SkillId, g.Key.DeptName, Count = g.Count() }
    ).ToListAsync(ct);

    // Total employees per department
    var totals = await (
        from u in _db.Users
        join d in _db.Departments on u.DepartmentId equals d.Id
        where deptNames.Contains(d.Name)
        group u by d.Name into g
        select new { Department = g.Key, Total = g.Count() }
    ).ToListAsync(ct);

    return required.Select(req =>
    {
      var cov = coverage.FirstOrDefault(c => c.SkillId == req.SkillId && c.DeptName == req.DepartmentName);
      var tot = totals.FirstOrDefault(t => t.Department == req.DepartmentName);
      return new SkillGapRawDto(req.Skill!.Name, cov?.Count ?? 0, tot?.Total ?? 0);
    }).ToList();
  }

  // ── US-6.4 ────────────────────────────────────────────────────────────────

  public async Task<List<RawUploadPointDto>> GetUploadCountsByDateAsync(
      DateTime from, CancellationToken ct = default)
  {
    // Bring to memory first — Date truncation is safer in-process
    var docs = await _db.Documents
        .Where(d => d.UploadedAt >= from && d.IsActive)
        .Select(d => new { d.UploadedAt, d.DocumentType })
        .ToListAsync(ct);

    return docs
        .GroupBy(d => new { Date = d.UploadedAt.Date, d.DocumentType })
        .Select(g => new RawUploadPointDto(g.Key.Date, g.Key.DocumentType, g.Count()))
        .ToList();
  }

  // ── US-6.5 ────────────────────────────────────────────────────────────────

  public async Task<List<ProficiencyDistributionDto>> GetProficiencyDistributionAsync(
      CancellationToken ct = default)
  {
    var data = await _db.EmployeeSkills
        .GroupBy(es => es.ProficiencyLevel)
        .Select(g => new { Level = g.Key, Count = g.Count() })
        .ToListAsync(ct);

    return data
        .OrderBy(d => (int)d.Level)
        .Select(d => new ProficiencyDistributionDto(d.Level.ToString(), d.Count))
        .ToList();
  }
}
