using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Infrastructure.Identity;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;

  public AdminController(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public record ChangeRoleRequest(string Role);
  public record UserSummary(Guid Id, string Email, string FullName, string Role, bool IsActive);

  [HttpPut("users/{id:guid}/role")]
  public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
  {
    if (request.Role is not ("Admin" or "User"))
      return BadRequest(new { message = "Role must be 'Admin' or 'User'." });

    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

    if (currentUserId == id.ToString())
      return BadRequest(new { message = "You cannot change your own role." });

    var user = await _userManager.FindByIdAsync(id.ToString());
    if (user is null)
      return NotFound(new { message = "User not found." });

    var currentRoles = await _userManager.GetRolesAsync(user);
    await _userManager.RemoveFromRolesAsync(user, currentRoles);
    await _userManager.AddToRoleAsync(user, request.Role);

    var roles = await _userManager.GetRolesAsync(user);
    return Ok(new UserSummary(user.Id, user.Email!, user.FullName, roles.FirstOrDefault() ?? request.Role, user.IsActive));
  }
}
