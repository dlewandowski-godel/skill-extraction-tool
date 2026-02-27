using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.Documents.Queries.GetMyDocuments;

public record DocumentDto(
    Guid DocumentId,
    string DocumentType,
    string FileName,
    string Status,
    DateTime UploadedAt,
    DateTime? ProcessedAt,
    string? ErrorMessage);

public record GetMyDocumentsQuery(Guid UserId) : IRequest<List<DocumentDto>>;

public class GetMyDocumentsQueryHandler : IRequestHandler<GetMyDocumentsQuery, List<DocumentDto>>
{
  private readonly IDocumentRepository _documentRepository;

  public GetMyDocumentsQueryHandler(IDocumentRepository documentRepository)
  {
    _documentRepository = documentRepository;
  }

  public async Task<List<DocumentDto>> Handle(
      GetMyDocumentsQuery request,
      CancellationToken cancellationToken)
  {
    var documents = await _documentRepository.GetByUserAsync(request.UserId, cancellationToken);

    return documents
        .Select(d => new DocumentDto(
            DocumentId: d.Id,
            DocumentType: d.DocumentType.ToString(),
            FileName: d.FileName,
            Status: d.Status.ToString(),
            UploadedAt: d.UploadedAt,
            ProcessedAt: d.ProcessedAt,
            ErrorMessage: d.ErrorMessage))
        .ToList();
  }
}
