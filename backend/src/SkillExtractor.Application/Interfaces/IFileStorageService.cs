using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Application.Interfaces;

public interface IFileStorageService
{
  /// <summary>
  /// Saves the file content and returns the full file path where it was stored.
  /// </summary>
  Task<string> SaveAsync(
      Guid userId,
      Stream fileContent,
      DocumentType documentType,
      CancellationToken cancellationToken = default);

  /// <summary>
  /// Deletes the file at the given path. Returns true if deleted, false if not found.
  /// </summary>
  Task<bool> DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}
