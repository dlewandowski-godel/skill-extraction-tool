using MediatR;
using SkillExtractor.Application.Documents.Commands.ProcessDocument;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Application.Documents.Commands.UploadDocument;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, UploadDocumentCommandResult>
{
  private readonly IDocumentRepository _documentRepository;
  private readonly IFileStorageService _fileStorageService;
  private readonly IMediator _mediator;

  public UploadDocumentCommandHandler(
      IDocumentRepository documentRepository,
      IFileStorageService fileStorageService,
      IMediator mediator)
  {
    _documentRepository = documentRepository;
    _fileStorageService = fileStorageService;
    _mediator = mediator;
  }

  public async Task<UploadDocumentCommandResult> Handle(
      UploadDocumentCommand command,
      CancellationToken cancellationToken)
  {
    // Deactivate any previously active document of the same type for this user
    var existingDocs = await _documentRepository.GetActiveByUserAndTypeAsync(
        command.UserId, command.DocumentType, cancellationToken);

    foreach (var existing in existingDocs)
      existing.Deactivate();

    // Persist the file and get the storage path
    var filePath = await _fileStorageService.SaveAsync(
        command.UserId,
        command.FileContent,
        command.DocumentType,
        cancellationToken);

    // Create and persist the document record
    var document = Document.Create(
        command.UserId,
        command.FileName,
        filePath,
        command.DocumentType);

    await _documentRepository.AddAsync(document, cancellationToken);
    await _documentRepository.SaveChangesAsync(cancellationToken);

    // Dispatch processing pipeline (Epic 4)
    await _mediator.Send(new ProcessDocumentCommand(document.Id), cancellationToken);

    return new UploadDocumentCommandResult(document.Id, document.Status.ToString());
  }
}
