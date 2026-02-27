using MediatR;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Application.Documents.Commands.UploadDocument;

public record UploadDocumentCommandResult(Guid DocumentId, string Status);

public record UploadDocumentCommand(
    Guid UserId,
    string FileName,
    string ContentType,
    long FileSize,
    Stream FileContent,
    DocumentType DocumentType) : IRequest<UploadDocumentCommandResult>;
