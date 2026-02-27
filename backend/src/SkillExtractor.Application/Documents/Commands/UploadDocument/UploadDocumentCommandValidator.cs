using FluentValidation;

namespace SkillExtractor.Application.Documents.Commands.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
  private const long MaxFileSizeBytes = 10L * 1024 * 1024; // 10 MB
  private static readonly byte[] PdfMagicBytes = [0x25, 0x50, 0x44, 0x46]; // %PDF

  public UploadDocumentCommandValidator()
  {
    RuleFor(x => x.FileSize)
        .GreaterThan(0)
            .WithMessage("Uploaded file is empty")
        .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage("File size must not exceed 10 MB");

    RuleFor(x => x.ContentType)
        .Equal("application/pdf")
            .WithMessage("Only PDF files are accepted");

    // Magic bytes check — only applied when MIME is pdf and file is non-empty
    RuleFor(x => x)
        .MustAsync(HasValidPdfMagicBytesAsync)
            .WithMessage("Only PDF files are accepted")
        .When(x => x.ContentType == "application/pdf" && x.FileSize > 0);
  }

  private static async Task<bool> HasValidPdfMagicBytesAsync(
      UploadDocumentCommand command,
      CancellationToken cancellationToken)
  {
    if (command.FileContent is null || !command.FileContent.CanRead)
      return false;

    var buffer = new byte[4];
    var read = await command.FileContent.ReadAsync(buffer.AsMemory(0, 4), cancellationToken);

    // Reset stream position for downstream use
    if (command.FileContent.CanSeek)
      command.FileContent.Seek(0, SeekOrigin.Begin);

    return read == 4 && buffer.SequenceEqual(PdfMagicBytes);
  }
}
