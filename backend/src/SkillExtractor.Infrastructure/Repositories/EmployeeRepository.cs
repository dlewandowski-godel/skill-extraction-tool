using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Common;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
  private readonly AppDbContext _db;

  public EmployeeRepository(AppDbContext db) => _db = db;

  public async Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(
      string? search,
      string? department,
      int page,
      int pageSize,
      CancellationToken ct = default)
  {
    IQueryable<ApplicationUser> query = _db.Users.Include(u => u.Department);

    if (!string.IsNullOrWhiteSpace(search))
    {
      var s = search.ToLower();
      query = query.Where(u =>
          u.FirstName.ToLower().Contains(s) ||
          u.LastName.ToLower().Contains(s) ||
          u.Email!.ToLower().Contains(s));
    }

    if (!string.IsNullOrWhiteSpace(department))
    {
      var dept = department.ToLower();
      query = query.Where(u => u.Department != null && u.Department.Name.ToLower() == dept);
    }

    var totalCount = await query.CountAsync(ct);

    var users = await query
        .OrderBy(u => u.LastName)
        .ThenBy(u => u.FirstName)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    var userIds = users.Select(u => u.Id).ToList();

    var lastUploads = await _db.Documents
        .Where(d => d.IsActive && userIds.Contains(d.UserId))
        .GroupBy(d => d.UserId)
        .Select(g => new { UserId = g.Key, LastUpload = g.Max(d => (DateTime?)d.UploadedAt) })
        .ToListAsync(ct);

    var roles = await _db.Set<IdentityUserRole<Guid>>()
        .Where(ur => userIds.Contains(ur.UserId))
        .Join(_db.Roles, ur => ur.RoleId, r => r.Id,
            (ur, r) => new { ur.UserId, RoleName = r.Name ?? "User" })
        .ToListAsync(ct);

    var items = users.Select(u => new EmployeeListItemDto(
        u.Id,
        u.FirstName,
        u.LastName,
        u.Email ?? string.Empty,
        u.Department?.Name,
        roles.FirstOrDefault(r => r.UserId == u.Id)?.RoleName ?? "User",
        u.IsActive,
        lastUploads.FirstOrDefault(l => l.UserId == u.Id)?.LastUpload
    )).ToList();

    return new PagedResult<EmployeeListItemDto>(items, totalCount, page, pageSize);
  }
}
