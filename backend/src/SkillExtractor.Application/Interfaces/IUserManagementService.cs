namespace SkillExtractor.Application.Interfaces;

public record CreateUserResult(bool Succeeded, Guid? UserId, string? TempPassword, string? Error);
public record UserOperationResult(bool Succeeded, string? Error);

public interface IUserManagementService
{
  Task<CreateUserResult> CreateUserAsync(
      string firstName, string lastName, string email, string role, Guid? departmentId,
      CancellationToken ct = default);

  Task<UserOperationResult> UpdateUserAsync(
      Guid userId, string firstName, string lastName, Guid? departmentId, string role, Guid callerId,
      CancellationToken ct = default);

  Task<UserOperationResult> DeactivateUserAsync(Guid userId, Guid adminId, CancellationToken ct = default);

  Task<UserOperationResult> ActivateUserAsync(Guid userId, CancellationToken ct = default);
}
