using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
  private readonly AppDbContext _db;

  public DocumentRepository(AppDbContext db)
  {
    _db = db;
  }

  public async Task AddAsync(Document document, CancellationToken cancellationToken = default)
  {
    await _db.Documents.AddAsync(document, cancellationToken);
  }

  public Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return _db.Documents.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
  }

  public Task<List<Document>> GetActiveByUserAndTypeAsync(
      Guid userId,
      DocumentType documentType,
      CancellationToken cancellationToken = default)
  {
    return _db.Documents
        .Where(d => d.UserId == userId && d.DocumentType == documentType && d.IsActive)
        .ToListAsync(cancellationToken);
  }

  public Task<List<Document>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return _db.Documents
        .Where(d => d.UserId == userId)
        .OrderByDescending(d => d.UploadedAt)
        .ToListAsync(cancellationToken);
  }

  public Task SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return _db.SaveChangesAsync(cancellationToken);
  }
}
