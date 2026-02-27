using MediatR;
using Microsoft.Extensions.Logging;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Application.Documents.Commands.ProcessDocument;

/// <summary>
/// Dispatched after a document is uploaded. Orchestrates the full extraction pipeline:
/// text extraction → skill matching → proficiency inference → persistence.
/// </summary>
public record ProcessDocumentCommand(Guid DocumentId) : IRequest;

public class ProcessDocumentCommandHandler : IRequestHandler<ProcessDocumentCommand>
{
  private readonly IDocumentRepository _documentRepository;
  private readonly IEmployeeSkillRepository _skillRepository;
  private readonly IPdfTextExtractor _pdfTextExtractor;
  private readonly ISkillExtractor _skillExtractor;
  private readonly IProficiencyInferenceService _proficiencyService;
  private readonly ILogger<ProcessDocumentCommandHandler> _logger;

  public ProcessDocumentCommandHandler(
      IDocumentRepository documentRepository,
      IEmployeeSkillRepository skillRepository,
      IPdfTextExtractor pdfTextExtractor,
      ISkillExtractor skillExtractor,
      IProficiencyInferenceService proficiencyService,
      ILogger<ProcessDocumentCommandHandler> logger)
  {
    _documentRepository = documentRepository;
    _skillRepository = skillRepository;
    _pdfTextExtractor = pdfTextExtractor;
    _skillExtractor = skillExtractor;
    _proficiencyService = proficiencyService;
    _logger = logger;
  }

  public async Task Handle(ProcessDocumentCommand request, CancellationToken cancellationToken)
  {
    // 1. Load document
    var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
    if (document is null)
    {
      _logger.LogWarning("ProcessDocumentCommand: document {DocumentId} not found. Skipping.", request.DocumentId);
      return;
    }

    // 2. Set → Processing
    document.SetProcessing();
    await _documentRepository.SaveChangesAsync(cancellationToken);

    try
    {
      _logger.LogInformation("Processing document {DocumentId} (path: {FilePath})",
          document.Id, document.FilePath);

      // 3. Extract text
      var text = _pdfTextExtractor.ExtractText(document.FilePath);

      // 4. Extract skills
      var extractedSkills = _skillExtractor.ExtractSkills(text);
      _logger.LogInformation("Extracted {Count} raw skills from document {DocumentId}",
          extractedSkills.Count, document.Id);

      // 5. Infer proficiency
      var skillsWithProficiency = _proficiencyService.InferProficiency(text, extractedSkills);

      // 6. Persist — replace old auto-extracted skills from this doc type, then merge
      await _skillRepository.DeleteAutoExtractedByDocumentTypeAsync(
          document.UserId, document.DocumentType, cancellationToken);

      // Load remaining skills for this user (from the other doc type, possibly)
      var existingSkills = await _skillRepository.GetByUserAsync(document.UserId, cancellationToken);
      var existingBySkillId = existingSkills.ToDictionary(e => e.SkillId);

      var toInsert = new List<EmployeeSkill>();
      foreach (var s in skillsWithProficiency)
      {
        if (existingBySkillId.TryGetValue(s.SkillId, out var existing))
        {
          // Never overwrite a manual admin override
          if (existing.IsManualOverride) continue;

          // Merge: keep the higher proficiency on the existing auto-extracted record
          if (s.ProficiencyLevel > existing.ProficiencyLevel)
            existing.UpdateAutoExtractedProficiency(s.ProficiencyLevel);
          // If existing is already higher or equal, no-op
        }
        else
        {
          toInsert.Add(EmployeeSkill.Create(
              document.UserId, s.SkillId, s.ProficiencyLevel, document.Id, document.DocumentType));
        }
      }

      _skillRepository.AddRange(toInsert);
      await _skillRepository.SaveChangesAsync(cancellationToken);

      // 7. Set → Done
      document.SetDone();
      await _documentRepository.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Document {DocumentId} processed successfully. Persisted {Count} skills.",
          document.Id, toInsert.Count);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to process document {DocumentId}", document.Id);

      // Fail gracefully — do NOT rethrow
      document.SetFailed(ex.Message);
      await _documentRepository.SaveChangesAsync(cancellationToken);
    }
  }
}
