namespace SkillExtractor.Application.Interfaces;

public record UserProfileInfo(Guid UserId, string FullName, string? Department);

public interface IUserRepository
{
  Task<UserProfileInfo?> GetProfileInfoAsync(Guid userId, CancellationToken ct = default);
}
