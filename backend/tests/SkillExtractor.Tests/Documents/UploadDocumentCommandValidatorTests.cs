using FluentAssertions;
using SkillExtractor.Application.Documents.Commands.UploadDocument;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Tests.Documents;

public class UploadDocumentCommandValidatorTests
{
  private readonly UploadDocumentCommandValidator _sut = new();

  private static readonly byte[] ValidPdfMagic = [0x25, 0x50, 0x44, 0x46]; // %PDF

  // ── Valid file ────────────────────────────────────────────────────────────

  [Fact]
  public async Task Validate_ValidPdfWithCorrectMagicBytes_PassesValidation()
  {
    var command = BuildCommand(
        contentType: "application/pdf",
        fileSize: 1024,
        magic: ValidPdfMagic);

    var result = await _sut.ValidateAsync(command);

    result.IsValid.Should().BeTrue();
  }

  // ── MIME type checks ─────────────────────────────────────────────────────

  [Theory]
  [InlineData("image/png")]
  [InlineData("application/octet-stream")]
  [InlineData("text/plain")]
  [InlineData("application/msword")]
  public async Task Validate_NonPdfMimeType_FailsWithOnlyPdfMessage(string contentType)
  {
    var command = BuildCommand(
        contentType: contentType,
        fileSize: 1024,
        magic: ValidPdfMagic);

    var result = await _sut.ValidateAsync(command);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.ErrorMessage == "Only PDF files are accepted");
  }

  [Fact]
  public async Task Validate_CorrectMimeTypeButWrongMagicBytes_FailsWithOnlyPdfMessage()
  {
    // MIME says PDF, but magic bytes say otherwise
    var command = BuildCommand(
        contentType: "application/pdf",
        fileSize: 1024,
        magic: [0x89, 0x50, 0x4E, 0x47]); // PNG magic bytes

    var result = await _sut.ValidateAsync(command);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.ErrorMessage == "Only PDF files are accepted");
  }

  // ── Size checks ───────────────────────────────────────────────────────────

  [Fact]
  public async Task Validate_FileSizeExceeds10Mb_FailsWithSizeMessage()
  {
    const long elevenMb = 11L * 1024 * 1024;
    var command = BuildCommand(
        contentType: "application/pdf",
        fileSize: elevenMb,
        magic: ValidPdfMagic,
        contentBytes: elevenMb);

    var result = await _sut.ValidateAsync(command);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.ErrorMessage == "File size must not exceed 10 MB");
  }

  [Fact]
  public async Task Validate_FileSizeExactly10Mb_PassesValidation()
  {
    const long tenMb = 10L * 1024 * 1024;
    var command = BuildCommand(
        contentType: "application/pdf",
        fileSize: tenMb,
        magic: ValidPdfMagic);

    var result = await _sut.ValidateAsync(command);

    result.IsValid.Should().BeTrue();
  }

  // ── Empty file check ─────────────────────────────────────────────────────

  [Fact]
  public async Task Validate_EmptyFile_FailsWithEmptyFileMessage()
  {
    var command = BuildCommand(
        contentType: "application/pdf",
        fileSize: 0,
        magic: []);

    var result = await _sut.ValidateAsync(command);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.ErrorMessage == "Uploaded file is empty");
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  private static UploadDocumentCommand BuildCommand(
      string contentType,
      long fileSize,
      byte[] magic,
      long? contentBytes = null)
  {
    // Build a stream with the magic bytes header plus padding to match fileSize
    var totalBytes = (int)Math.Min(contentBytes ?? fileSize, 64); // cap stream size for tests
    var streamContent = new byte[Math.Max(totalBytes, magic.Length)];
    Array.Copy(magic, streamContent, Math.Min(magic.Length, streamContent.Length));

    return new UploadDocumentCommand(
        UserId: Guid.NewGuid(),
        FileName: "test.pdf",
        ContentType: contentType,
        FileSize: fileSize,
        FileContent: new MemoryStream(streamContent),
        DocumentType: DocumentType.CV);
  }
}
