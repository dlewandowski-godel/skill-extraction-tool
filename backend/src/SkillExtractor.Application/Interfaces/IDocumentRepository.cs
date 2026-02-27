using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Application.Interfaces;

public interface IDocumentRepository
{
  Task AddAsync(Document document, CancellationToken cancellationToken = default);
  Task<List<Document>> GetActiveByUserAndTypeAsync(Guid userId, DocumentType documentType, CancellationToken cancellationToken = default);
  Task<List<Document>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
