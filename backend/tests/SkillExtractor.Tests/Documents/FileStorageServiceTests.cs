using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Infrastructure.Services;

namespace SkillExtractor.Tests.Documents;

public class FileStorageServiceTests : IDisposable
{
  private readonly string _tempDir;
  private readonly FileStorageService _sut;

  public FileStorageServiceTests()
  {
    // Use an isolated temp directory per test class instance
    _tempDir = Path.Combine(Path.GetTempPath(), "SkillExtractorTests", Guid.NewGuid().ToString());
    Directory.CreateDirectory(_tempDir);

    var configuration = Substitute.For<IConfiguration>();
    configuration["FILE_STORAGE_PATH"].Returns(_tempDir);

    _sut = new FileStorageService(configuration);
  }

  // ── SaveAsync ─────────────────────────────────────────────────────────────

  [Fact]
  public async Task SaveAsync_WritesFileToExpectedPathPattern()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var content = new MemoryStream([0x25, 0x50, 0x44, 0x46, 0x00]); // %PDF

    // Act
    var filePath = await _sut.SaveAsync(userId, content, DocumentType.CV);

    // Assert — path matches {basePath}/{userId}/{guid}-CV.pdf
    filePath.Should().StartWith(Path.Combine(_tempDir, userId.ToString()));
    filePath.Should().EndWith("-CV.pdf");
    File.Exists(filePath).Should().BeTrue();
  }

  [Fact]
  public async Task SaveAsync_CreatesUserDirectoryIfNotExists()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var userDir = Path.Combine(_tempDir, userId.ToString());

    // Ensure user dir doesn't exist yet
    Directory.Exists(userDir).Should().BeFalse();

    var content = new MemoryStream([0x25, 0x50, 0x44, 0x46]);

    // Act
    await _sut.SaveAsync(userId, content, DocumentType.IFU);

    // Assert
    Directory.Exists(userDir).Should().BeTrue();
  }

  [Fact]
  public async Task SaveAsync_WritesCorrectBytesToFile()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var bytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x01, 0x02, 0x03 };
    var content = new MemoryStream(bytes);

    // Act
    var filePath = await _sut.SaveAsync(userId, content, DocumentType.CV);

    // Assert
    var written = await File.ReadAllBytesAsync(filePath);
    written.Should().Equal(bytes);
  }

  [Fact]
  public async Task SaveAsync_EachCallCreatesUniqueFileName()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var content1 = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
    var content2 = new MemoryStream([0x25, 0x50, 0x44, 0x46]);

    // Act
    var path1 = await _sut.SaveAsync(userId, content1, DocumentType.CV);
    var path2 = await _sut.SaveAsync(userId, content2, DocumentType.CV);

    // Assert
    path1.Should().NotBe(path2);
  }

  // ── DeleteAsync ───────────────────────────────────────────────────────────

  [Fact]
  public async Task DeleteAsync_ExistingFile_DeletesFileAndReturnsTrue()
  {
    // Arrange — create a file to delete
    var userId = Guid.NewGuid();
    var content = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
    var filePath = await _sut.SaveAsync(userId, content, DocumentType.CV);

    File.Exists(filePath).Should().BeTrue();

    // Act
    var deleted = await _sut.DeleteAsync(filePath);

    // Assert
    deleted.Should().BeTrue();
    File.Exists(filePath).Should().BeFalse();
  }

  [Fact]
  public async Task DeleteAsync_NonExistentFile_ReturnsFalseWithoutException()
  {
    // Arrange
    var nonExistentPath = Path.Combine(_tempDir, "ghost-file.pdf");

    // Act
    var result = await _sut.DeleteAsync(nonExistentPath);

    // Assert — no exception, returns false
    result.Should().BeFalse();
  }

  // ── Cleanup ───────────────────────────────────────────────────────────────

  public void Dispose()
  {
    if (Directory.Exists(_tempDir))
      Directory.Delete(_tempDir, recursive: true);
  }
}
