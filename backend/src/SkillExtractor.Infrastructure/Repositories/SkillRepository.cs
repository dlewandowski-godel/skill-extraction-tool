using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
  private readonly AppDbContext _db;

  public SkillRepository(AppDbContext db) => _db = db;

  public Task<List<Skill>> GetAllActiveAsync(CancellationToken ct = default)
      => _db.Skills.Where(s => s.IsActive).ToListAsync(ct);

  public Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default)
      => _db.Skills.FirstOrDefaultAsync(s => s.Id == id, ct);

  public async Task AddAsync(Skill skill, CancellationToken ct = default)
      => await _db.Skills.AddAsync(skill, ct);

  public Task SaveChangesAsync(CancellationToken ct = default)
      => _db.SaveChangesAsync(ct);
}
