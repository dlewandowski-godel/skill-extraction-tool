namespace SkillExtractor.Application.Interfaces;

/// <summary>
/// Extracts plain text from a PDF file at the given file path.
/// </summary>
public interface IPdfTextExtractor
{
  /// <summary>
  /// Reads all text from the PDF at <paramref name="filePath"/>, respecting reading order.
  /// Returns an empty string for empty, corrupt, or password-protected PDFs (never throws).
  /// </summary>
  string ExtractText(string filePath);
}
