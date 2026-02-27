using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Interfaces;

public interface ISkillRepository
{
  Task<List<Skill>> GetAllActiveAsync(CancellationToken ct = default);
  Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default);
  Task AddAsync(Skill skill, CancellationToken ct = default);
  Task SaveChangesAsync(CancellationToken ct = default);
}
