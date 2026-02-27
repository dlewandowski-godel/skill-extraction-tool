using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.EmployeeManagement.Commands.ActivateEmployee;
using SkillExtractor.Application.EmployeeManagement.Commands.CreateEmployee;
using SkillExtractor.Application.EmployeeManagement.Commands.DeactivateEmployee;
using SkillExtractor.Application.EmployeeManagement.Commands.EditEmployee;
using SkillExtractor.Application.EmployeeManagement.Queries.GetEmployees;
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

  // ── Employee Management (US-7.1 / 7.2 / 7.3 / 7.4) ────────────────────────

  /// <summary>GET /api/admin/employees?page=1&pageSize=20&search=john&department=Engineering (US-7.1)</summary>
  [HttpGet("employees")]
  public async Task<IActionResult> GetEmployees(
      [FromQuery] string? search,
      [FromQuery] string? department,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 20,
      CancellationToken cancellationToken = default)
  {
    var result = await _mediator.Send(
        new GetEmployeesQuery(search, department, page, pageSize), cancellationToken);
    return Ok(result);
  }

  public record CreateEmployeeRequest(
      string FirstName, string LastName, string Email, string Role, Guid? DepartmentId);

  /// <summary>POST /api/admin/employees (US-7.2)</summary>
  [HttpPost("employees")]
  public async Task<IActionResult> CreateEmployee(
      [FromBody] CreateEmployeeRequest request,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new CreateEmployeeCommand(
            request.FirstName, request.LastName, request.Email,
            request.Role, request.DepartmentId),
        cancellationToken);

    if (!result.Succeeded)
      return BadRequest(new { message = result.Error });

    return Ok(new
    {
      employeeId = result.EmployeeId,
      tempPassword = result.TempPassword,
      message = $"Employee account created. Temporary password: {result.TempPassword}",
    });
  }

  public record EditEmployeeRequest(
      string FirstName, string LastName, Guid? DepartmentId, string Role);

  /// <summary>PUT /api/admin/employees/{id} (US-7.3)</summary>
  [HttpPut("employees/{id:guid}")]
  public async Task<IActionResult> EditEmployee(
      Guid id,
      [FromBody] EditEmployeeRequest request,
      CancellationToken cancellationToken)
  {
    var callerId = GetUserId() ?? Guid.Empty;
    var result = await _mediator.Send(
        new EditEmployeeCommand(id, callerId, request.FirstName, request.LastName,
            request.DepartmentId, request.Role),
        cancellationToken);

    return result switch
    {
      EditEmployeeResult.Ok => Ok(new { message = "Employee updated." }),
      EditEmployeeResult.NotFound => NotFound(new { message = "Employee not found." }),
      EditEmployeeResult.CannotChangeOwnRole => BadRequest(new { message = "You cannot change your own role." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>PUT /api/admin/employees/{id}/deactivate (US-7.4)</summary>
  [HttpPut("employees/{id:guid}/deactivate")]
  public async Task<IActionResult> DeactivateEmployee(Guid id, CancellationToken cancellationToken)
  {
    var adminId = GetUserId() ?? Guid.Empty;
    var result = await _mediator.Send(new DeactivateEmployeeCommand(id, adminId), cancellationToken);

    return result switch
    {
      DeactivateEmployeeResult.Ok => Ok(new { message = "Employee deactivated." }),
      DeactivateEmployeeResult.NotFound => NotFound(new { message = "Employee not found." }),
      DeactivateEmployeeResult.CannotDeactivateSelf => BadRequest(new { message = "You cannot deactivate your own account." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>PUT /api/admin/employees/{id}/activate (US-7.4)</summary>
  [HttpPut("employees/{id:guid}/activate")]
  public async Task<IActionResult> ActivateEmployee(Guid id, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ActivateEmployeeCommand(id), cancellationToken);

    return result switch
    {
      ActivateEmployeeResult.Ok => Ok(new { message = "Employee activated." }),
      ActivateEmployeeResult.NotFound => NotFound(new { message = "Employee not found." }),
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
