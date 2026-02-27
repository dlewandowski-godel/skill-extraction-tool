using Microsoft.Extensions.Configuration;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
  private readonly string _basePath;

  public FileStorageService(IConfiguration configuration)
  {
    _basePath = configuration["FILE_STORAGE_PATH"] ?? "/uploads";
    Directory.CreateDirectory(_basePath);
  }

  public async Task<string> SaveAsync(
      Guid userId,
      Stream fileContent,
      DocumentType documentType,
      CancellationToken cancellationToken = default)
  {
    var userDir = Path.Combine(_basePath, userId.ToString());
    Directory.CreateDirectory(userDir);

    var guid = Guid.NewGuid();
    var fileName = $"{guid}-{documentType}.pdf";
    var fullPath = Path.Combine(userDir, fileName);

    await using var fs = new FileStream(
        fullPath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None,
        bufferSize: 81920,
        useAsync: true);

    await fileContent.CopyToAsync(fs, cancellationToken);

    return fullPath;
  }

  public Task<bool> DeleteAsync(string filePath, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(filePath))
      return Task.FromResult(false);

    File.Delete(filePath);
    return Task.FromResult(true);
  }
}
