using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Domain.Entities;

public class Document
{
  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public string FileName { get; private set; } = string.Empty;
  public string FilePath { get; private set; } = string.Empty;
  public DocumentType DocumentType { get; private set; }
  public DocumentStatus Status { get; private set; }
  public DateTime UploadedAt { get; private set; }
  public DateTime? ProcessedAt { get; private set; }
  public string? ErrorMessage { get; private set; }
  public bool IsActive { get; private set; } = true;

  // EF Core requires a parameterless constructor
  private Document() { }

  public static Document Create(
      Guid userId,
      string fileName,
      string filePath,
      DocumentType documentType)
  {
    return new Document
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      FileName = fileName,
      FilePath = filePath,
      DocumentType = documentType,
      Status = DocumentStatus.Pending,
      UploadedAt = DateTime.UtcNow,
      IsActive = true,
    };
  }

  public void SetProcessing()
  {
    Status = DocumentStatus.Processing;
  }

  public void SetDone()
  {
    Status = DocumentStatus.Done;
    ProcessedAt = DateTime.UtcNow;
  }

  public void SetFailed(string errorMessage)
  {
    Status = DocumentStatus.Failed;
    ErrorMessage = errorMessage;
    ProcessedAt = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    IsActive = false;
  }
}
