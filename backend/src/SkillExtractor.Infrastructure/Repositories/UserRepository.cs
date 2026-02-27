using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
  private readonly AppDbContext _db;
  private readonly UserManager<ApplicationUser> _userManager;

  public UserRepository(AppDbContext db, UserManager<ApplicationUser> userManager)
  {
    _db = db;
    _userManager = userManager;
  }

  public async Task<UserProfileInfo?> GetProfileInfoAsync(Guid userId, CancellationToken ct = default)
  {
    var user = await _db.Users
        .Include(u => u.Department)
        .FirstOrDefaultAsync(u => u.Id == userId, ct);
    if (user is null) return null;

    var roles = await _userManager.GetRolesAsync(user);
    var role = roles.FirstOrDefault();

    return new UserProfileInfo(userId, user.FullName, user.FirstName, user.LastName, user.Department?.Name, user.DepartmentId, role, user.IsActive);
  }
}
