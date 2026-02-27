using System.Text;

namespace SkillExtractor.Tests.Fixtures;

/// <summary>
/// Generates minimal valid PDF files in memory for unit tests.
/// </summary>
internal static class TestPdfFactory
{
  /// <summary>Creates a valid single-page PDF containing the given text.</summary>
  public static byte[] CreateSinglePagePdf(string bodyText = "Python machine learning expert advanced C#")
  {
    var streamContent = $"BT /F1 12 Tf 72 720 Td ({EscapePdfString(bodyText)}) Tj ET";
    return BuildPdf(streamContent);
  }

  /// <summary>Creates a valid two-page PDF, each page containing the supplied text.</summary>
  public static byte[] CreateTwoPagePdf(string page1Text, string page2Text)
  {
    // Build two content streams via a simple multi-page PDF
    var contentStream1 = $"BT /F1 12 Tf 72 720 Td ({EscapePdfString(page1Text)}) Tj ET";
    var contentStream2 = $"BT /F1 12 Tf 72 720 Td ({EscapePdfString(page2Text)}) Tj ET";
    return BuildTwoPagePdf(contentStream1, contentStream2);
  }

  /// <summary>Creates an empty (0-byte) file.</summary>
  public static byte[] CreateEmptyFile() => Array.Empty<byte>();

  /// <summary>Creates a byte sequence that is not a valid PDF (corrupt).</summary>
  public static byte[] CreateCorruptPdf() => Encoding.ASCII.GetBytes("This is not a PDF file at all.");

  /// <summary>Writes a single-page PDF to a temp file and returns its path.</summary>
  public static string WriteTempPdf(string bodyText = "Python machine learning expert advanced C#")
  {
    var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
    File.WriteAllBytes(path, CreateSinglePagePdf(bodyText));
    return path;
  }

  /// <summary>Writes a two-page PDF to a temp file and returns its path.</summary>
  public static string WriteTwoPageTempPdf(string page1Text, string page2Text)
  {
    var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
    File.WriteAllBytes(path, CreateTwoPagePdf(page1Text, page2Text));
    return path;
  }

  // -----------------------------------------------------------------------
  // Internals
  // -----------------------------------------------------------------------

  private static string EscapePdfString(string text)
      => text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

  private static byte[] BuildPdf(string contentStream)
  {
    // Objects:
    // 1 -> Catalog
    // 2 -> Pages  (1 kid)
    // 3 -> Page
    // 4 -> Font
    // 5 -> Contents stream
    var streamLen = Encoding.ASCII.GetByteCount(contentStream);

    var obj1 = "1 0 obj\n<</Type /Catalog /Pages 2 0 R>>\nendobj\n";
    var obj2 = "2 0 obj\n<</Type /Pages /Kids [3 0 R] /Count 1>>\nendobj\n";
    var obj3 = "3 0 obj\n<</Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] " +
               "/Contents 5 0 R /Resources <</Font <</F1 4 0 R>>>>>>\nendobj\n";
    var obj4 = "4 0 obj\n<</Type /Font /Subtype /Type1 /BaseFont /Helvetica>>\nendobj\n";
    var obj5 = $"5 0 obj\n<</Length {streamLen}>>\nstream\n{contentStream}\nendstream\nendobj\n";

    return AssemblePdf(new[] { obj1, obj2, obj3, obj4, obj5 });
  }

  private static byte[] BuildTwoPagePdf(string contentStream1, string contentStream2)
  {
    var len1 = Encoding.ASCII.GetByteCount(contentStream1);
    var len2 = Encoding.ASCII.GetByteCount(contentStream2);

    var obj1 = "1 0 obj\n<</Type /Catalog /Pages 2 0 R>>\nendobj\n";
    var obj2 = "2 0 obj\n<</Type /Pages /Kids [3 0 R 4 0 R] /Count 2>>\nendobj\n";
    var obj3 = "3 0 obj\n<</Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] " +
               "/Contents 6 0 R /Resources <</Font <</F1 5 0 R>>>>>>\nendobj\n";
    var obj4 = "4 0 obj\n<</Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] " +
               "/Contents 7 0 R /Resources <</Font <</F1 5 0 R>>>>>>\nendobj\n";
    var obj5 = "5 0 obj\n<</Type /Font /Subtype /Type1 /BaseFont /Helvetica>>\nendobj\n";
    var obj6 = $"6 0 obj\n<</Length {len1}>>\nstream\n{contentStream1}\nendstream\nendobj\n";
    var obj7 = $"7 0 obj\n<</Length {len2}>>\nstream\n{contentStream2}\nendstream\nendobj\n";

    return AssemblePdf(new[] { obj1, obj2, obj3, obj4, obj5, obj6, obj7 });
  }

  private static byte[] AssemblePdf(string[] objects)
  {
    var header = "%PDF-1.4\n";
    var parts = new List<byte[]>();
    var offsets = new int[objects.Length];

    var headerBytes = Encoding.ASCII.GetBytes(header);
    parts.Add(headerBytes);
    var cursor = headerBytes.Length;

    for (var i = 0; i < objects.Length; i++)
    {
      offsets[i] = cursor;
      var objBytes = Encoding.ASCII.GetBytes(objects[i]);
      parts.Add(objBytes);
      cursor += objBytes.Length;
    }

    // Build xref
    var xrefOffset = cursor;
    var xrefSb = new StringBuilder();
    xrefSb.AppendLine("xref");
    xrefSb.AppendLine($"0 {objects.Length + 1}");
    xrefSb.AppendLine("0000000000 65535 f ");  // null object
    foreach (var off in offsets)
      xrefSb.AppendLine($"{off:D10} 00000 n ");

    xrefSb.AppendLine($"trailer\n<</Size {objects.Length + 1} /Root 1 0 R>>");
    xrefSb.AppendLine($"startxref\n{xrefOffset}");
    xrefSb.Append("%%EOF");

    parts.Add(Encoding.ASCII.GetBytes(xrefSb.ToString()));

    // Concatenate
    var total = parts.Sum(p => p.Length);
    var result = new byte[total];
    var pos = 0;
    foreach (var part in parts)
    {
      Buffer.BlockCopy(part, 0, result, pos, part.Length);
      pos += part.Length;
    }
    return result;
  }
}
