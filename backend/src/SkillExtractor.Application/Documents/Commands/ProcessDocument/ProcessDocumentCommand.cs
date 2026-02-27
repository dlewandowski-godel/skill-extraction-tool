using MediatR;

namespace SkillExtractor.Application.Documents.Commands.ProcessDocument;

/// <summary>
/// Dispatched after a document is uploaded. The actual extraction logic is implemented in Epic 4 (US-4.4).
/// </summary>
public record ProcessDocumentCommand(Guid DocumentId) : IRequest;

/// <summary>
/// Stub handler — will be replaced by the real extraction pipeline in US-4.4.
/// </summary>
public class ProcessDocumentCommandHandler : IRequestHandler<ProcessDocumentCommand>
{
  public Task Handle(ProcessDocumentCommand request, CancellationToken cancellationToken)
  {
    // No-op until Epic 4
    return Task.CompletedTask;
  }
}
