using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly AppDbContext _db;

  public UserManagementService(UserManager<ApplicationUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<CreateUserResult> CreateUserAsync(
      string firstName, string lastName, string email, string role,
      Guid? departmentId, CancellationToken ct = default)
  {
    var existing = await _userManager.FindByEmailAsync(email);
    if (existing is not null)
      return new CreateUserResult(false, null, null, "Email already in use.");

    var tempPassword = Guid.NewGuid().ToString("N")[..12] + "A1!";

    var user = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      UserName = email,
      Email = email,
      FirstName = firstName,
      LastName = lastName,
      IsActive = true,
      DepartmentId = departmentId,
      CreatedAt = DateTime.UtcNow,
    };

    var createResult = await _userManager.CreateAsync(user, tempPassword);
    if (!createResult.Succeeded)
    {
      var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
      return new CreateUserResult(false, null, null, errors);
    }

    var assignedRole = role is "Admin" or "User" ? role : "User";
    await _userManager.AddToRoleAsync(user, assignedRole);

    return new CreateUserResult(true, user.Id, tempPassword, null);
  }

  public async Task<UserOperationResult> UpdateUserAsync(
      Guid userId, string firstName, string lastName, Guid? departmentId,
      string role, Guid callerId, CancellationToken ct = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null) return new UserOperationResult(false, "NotFound");

    // Check if caller is trying to change their own role
    if (userId == callerId)
    {
      var callerRoles = await _userManager.GetRolesAsync(user);
      var currentRole = callerRoles.FirstOrDefault() ?? "User";
      if (currentRole != role)
        return new UserOperationResult(false, "CannotChangeOwnRole");
    }

    user.FirstName = firstName;
    user.LastName = lastName;
    user.DepartmentId = departmentId;

    await _userManager.UpdateAsync(user);

    // Role update (only when caller is not the target user)
    if (userId != callerId)
    {
      var currentRoles = await _userManager.GetRolesAsync(user);
      await _userManager.RemoveFromRolesAsync(user, currentRoles);
      var assignedRole = role is "Admin" or "User" ? role : "User";
      await _userManager.AddToRoleAsync(user, assignedRole);
    }

    return new UserOperationResult(true, null);
  }

  public async Task<UserOperationResult> DeactivateUserAsync(
      Guid userId, Guid adminId, CancellationToken ct = default)
  {
    if (userId == adminId)
      return new UserOperationResult(false, "CannotDeactivateSelf");

    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null) return new UserOperationResult(false, "NotFound");

    user.IsActive = false;
    await _userManager.UpdateAsync(user);

    // Revoke all active refresh tokens
    var tokens = await _db.RefreshTokens
        .Where(t => t.UserId == userId && !t.IsRevoked)
        .ToListAsync(ct);
    foreach (var t in tokens) t.Revoke();
    await _db.SaveChangesAsync(ct);

    return new UserOperationResult(true, null);
  }

  public async Task<UserOperationResult> ActivateUserAsync(
      Guid userId, CancellationToken ct = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null) return new UserOperationResult(false, "NotFound");

    user.IsActive = true;
    await _userManager.UpdateAsync(user);

    return new UserOperationResult(true, null);
  }
}
