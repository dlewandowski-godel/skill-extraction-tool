using Microsoft.Extensions.Logging;
using SkillExtractor.Application.Interfaces;
using UglyToad.PdfPig;

namespace SkillExtractor.Infrastructure.Services;

public class PdfPigTextExtractor : IPdfTextExtractor
{
  private readonly ILogger<PdfPigTextExtractor> _logger;

  public PdfPigTextExtractor(ILogger<PdfPigTextExtractor> logger)
  {
    _logger = logger;
  }

  public string ExtractText(string filePath)
  {
    try
    {
      using var document = PdfDocument.Open(filePath);
      var sb = new System.Text.StringBuilder();
      foreach (var page in document.GetPages())
      {
        // page.Text returns characters in presentation order (equivalent to
        // ContentOrderTextExtractor.GetText in earlier PdfPig versions)
        var text = page.Text;
        if (!string.IsNullOrEmpty(text))
          sb.AppendLine(text);
      }
      return sb.ToString();
    }
    catch (Exception ex) when (IsPasswordProtected(ex))
    {
      _logger.LogWarning("PDF is password protected: {FilePath}", filePath);
      return string.Empty;
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Could not read PDF (corrupt or unreadable): {FilePath}", filePath);
      return string.Empty;
    }
  }

  private static bool IsPasswordProtected(Exception ex)
  {
    var msg = ex.Message.ToLowerInvariant();
    return msg.Contains("password") || msg.Contains("encrypt") || ex.GetType().Name.Contains("Password");
  }
}
