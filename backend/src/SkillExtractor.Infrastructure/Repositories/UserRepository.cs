using Microsoft.AspNetCore.Identity;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Infrastructure.Identity;

namespace SkillExtractor.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
  private readonly UserManager<ApplicationUser> _userManager;

  public UserRepository(UserManager<ApplicationUser> userManager)
      => _userManager = userManager;

  public async Task<UserProfileInfo?> GetProfileInfoAsync(Guid userId, CancellationToken ct = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null) return null;

    return new UserProfileInfo(userId, user.FullName, null); // Department added in Epic 7
  }
}
