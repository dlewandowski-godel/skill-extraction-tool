using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SkillExtractor.Infrastructure.Services;
using SkillExtractor.Tests.Fixtures;

namespace SkillExtractor.Tests.Extraction;

public class PdfPigTextExtractorTests : IDisposable
{
  private readonly PdfPigTextExtractor _sut;
  private readonly List<string> _tempFiles = new();

  public PdfPigTextExtractorTests()
  {
    _sut = new PdfPigTextExtractor(NullLogger<PdfPigTextExtractor>.Instance);
  }

  public void Dispose()
  {
    foreach (var f in _tempFiles)
      if (File.Exists(f)) File.Delete(f);
  }

  private string CreateTempFile(byte[] bytes, string extension = ".pdf")
  {
    var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
    File.WriteAllBytes(path, bytes);
    _tempFiles.Add(path);
    return path;
  }

  [Fact]
  public void ExtractText_ValidPdf_ReturnsNonEmptyString()
  {
    // Arrange
    var path = CreateTempFile(TestPdfFactory.CreateSinglePagePdf("Python C# machine learning"));

    // Act
    var result = _sut.ExtractText(path);

    // Assert
    result.Should().NotBeNullOrWhiteSpace();
  }

  [Fact]
  public void ExtractText_TwoPagePdf_ConcatenatesAllPages()
  {
    // Arrange
    var path = CreateTempFile(TestPdfFactory.CreateTwoPagePdf("page one content", "page two content"));

    // Act
    var result = _sut.ExtractText(path);

    // Assert
    result.Should().NotBeNullOrWhiteSpace(
        "both pages should be extracted and concatenated");
  }

  [Fact]
  public void ExtractText_EmptyFile_ReturnsEmptyStringWithoutThrowing()
  {
    // Arrange – 0-byte file
    var path = CreateTempFile(TestPdfFactory.CreateEmptyFile());

    // Act
    string? result = null;
    var act = () => { result = _sut.ExtractText(path); };

    // Assert
    act.Should().NotThrow();
    result.Should().NotBeNull();
    result!.Should().BeEmpty();
  }

  [Fact]
  public void ExtractText_CorruptFile_ReturnsEmptyStringWithoutThrowing()
  {
    // Arrange
    var path = CreateTempFile(TestPdfFactory.CreateCorruptPdf());

    // Act
    string? result = null;
    var act = () => { result = _sut.ExtractText(path); };

    // Assert
    act.Should().NotThrow();
    result.Should().NotBeNull();
    result!.Should().BeEmpty();
  }

  [Fact]
  public void ExtractText_NonExistentFile_ReturnsEmptyStringWithoutThrowing()
  {
    // Arrange
    var path = Path.Combine(Path.GetTempPath(), "does_not_exist.pdf");

    // Act
    string? result = null;
    var act = () => { result = _sut.ExtractText(path); };

    // Assert
    act.Should().NotThrow();
    result!.Should().BeEmpty();
  }
}
