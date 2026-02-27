using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Profile.Commands.AddSkillToEmployee;
using SkillExtractor.Application.Profile.Commands.ChangeProficiency;
using SkillExtractor.Application.Profile.Commands.RemoveSkillFromEmployee;
using SkillExtractor.Application.Profile.Queries.GetEmployeeProfileById;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Infrastructure.Identity;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMediator _mediator;

  public AdminController(UserManager<ApplicationUser> userManager, IMediator mediator)
  {
    _userManager = userManager;
    _mediator = mediator;
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

  // ── Employee Profile (US-5.3) ─────────────────────────────────────────────

  /// <summary>GET /api/admin/employees/{id}/profile</summary>
  [HttpGet("employees/{id:guid}/profile")]
  public async Task<IActionResult> GetEmployeeProfile(Guid id, CancellationToken cancellationToken)
  {
    var profile = await _mediator.Send(new GetEmployeeProfileByIdQuery(id), cancellationToken);
    return profile is null ? NotFound(new { message = "Employee not found." }) : Ok(profile);
  }

  // ── Employee Skills CRUD (US-5.4 / 5.5 / 5.6) ────────────────────────────

  public record AddSkillRequest(Guid SkillId, string ProficiencyLevel);
  public record ChangeProficiencyRequest(string ProficiencyLevel);

  /// <summary>POST /api/admin/employees/{id}/skills — manually add or upsert a skill (US-5.4)</summary>
  [HttpPost("employees/{id:guid}/skills")]
  public async Task<IActionResult> AddSkill(
      Guid id,
      [FromBody] AddSkillRequest request,
      CancellationToken cancellationToken)
  {
    if (!Enum.TryParse<ProficiencyLevel>(request.ProficiencyLevel, ignoreCase: true, out var level))
      return BadRequest(new { message = $"Invalid proficiency level '{request.ProficiencyLevel}'. Allowed: Beginner, Intermediate, Advanced, Expert." });

    var adminId = GetUserId() ?? Guid.Empty;
    var result = await _mediator.Send(
        new AddSkillToEmployeeCommand(adminId, id, request.SkillId, level), cancellationToken);

    return result switch
    {
      AddSkillResult.Ok => Ok(new { message = "Skill added." }),
      AddSkillResult.SkillNotFound => BadRequest(new { message = "Skill not found in taxonomy." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>DELETE /api/admin/employees/{id}/skills/{skillId} — remove a skill (US-5.5)</summary>
  [HttpDelete("employees/{id:guid}/skills/{skillId:guid}")]
  public async Task<IActionResult> RemoveSkill(
      Guid id,
      Guid skillId,
      CancellationToken cancellationToken)
  {
    var adminId = GetUserId() ?? Guid.Empty;
    var result = await _mediator.Send(
        new RemoveSkillFromEmployeeCommand(adminId, id, skillId), cancellationToken);

    return result switch
    {
      RemoveSkillResult.Ok => NoContent(),
      RemoveSkillResult.NotFound => NotFound(new { message = "Skill not found on this employee's profile." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>PUT /api/admin/employees/{id}/skills/{skillId} — change proficiency level (US-5.6)</summary>
  [HttpPut("employees/{id:guid}/skills/{skillId:guid}")]
  public async Task<IActionResult> ChangeProficiency(
      Guid id,
      Guid skillId,
      [FromBody] ChangeProficiencyRequest request,
      CancellationToken cancellationToken)
  {
    if (!Enum.TryParse<ProficiencyLevel>(request.ProficiencyLevel, ignoreCase: true, out var level))
      return BadRequest(new { message = $"Invalid proficiency level '{request.ProficiencyLevel}'. Allowed: Beginner, Intermediate, Advanced, Expert." });

    var adminId = GetUserId() ?? Guid.Empty;
    var result = await _mediator.Send(
        new ChangeProficiencyCommand(adminId, id, skillId, level), cancellationToken);

    return result switch
    {
      ChangeProficiencyResult.Ok => Ok(new { message = "Proficiency updated." }),
      ChangeProficiencyResult.NotFound => NotFound(new { message = "Skill not found on this employee's profile." }),
      _ => StatusCode(500),
    };
  }

  private Guid? GetUserId()
  {
    var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");
    return Guid.TryParse(value, out var id) ? id : null;
  }
}
