namespace SkillExtractor.Application.Interfaces;

public record UserProfileInfo(
    Guid UserId,
    string FullName,
    string FirstName,
    string LastName,
    string? Department,
    Guid? DepartmentId,
    string? Role,
    bool IsActive);

public interface IUserRepository
{
  Task<UserProfileInfo?> GetProfileInfoAsync(Guid userId, CancellationToken ct = default);
}
