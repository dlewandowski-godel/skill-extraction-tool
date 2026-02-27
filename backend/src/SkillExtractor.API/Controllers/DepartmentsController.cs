using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Departments.Commands.CreateDepartment;
using SkillExtractor.Application.Departments.Commands.DeleteDepartment;
using SkillExtractor.Application.Departments.Commands.RenameDepartment;
using SkillExtractor.Application.Departments.Queries.GetDepartments;
using SkillExtractor.Application.Taxonomy.Commands.AddRequiredSkill;
using SkillExtractor.Application.Taxonomy.Commands.RemoveRequiredSkill;
using SkillExtractor.Application.Taxonomy.Queries.GetRequiredSkills;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/admin/departments")]
[Authorize(Roles = "Admin")]
public class DepartmentsController : ControllerBase
{
  private readonly IMediator _mediator;

  public DepartmentsController(IMediator mediator) => _mediator = mediator;

  /// <summary>GET /api/admin/departments</summary>
  [HttpGet]
  public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetDepartmentsQuery(), cancellationToken);
    return Ok(result);
  }

  public record CreateDepartmentRequest(string Name);

  /// <summary>POST /api/admin/departments</summary>
  [HttpPost]
  public async Task<IActionResult> Create(
      [FromBody] CreateDepartmentRequest request,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new CreateDepartmentCommand(request.Name), cancellationToken);

    if (!result.Succeeded)
      return BadRequest(new { message = result.Error });

    return Ok(new { departmentId = result.DepartmentId });
  }

  public record RenameDepartmentRequest(string Name);

  /// <summary>PUT /api/admin/departments/{id}</summary>
  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Rename(
      Guid id,
      [FromBody] RenameDepartmentRequest request,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new RenameDepartmentCommand(id, request.Name), cancellationToken);

    return result switch
    {
      RenameDepartmentResult.Ok => Ok(new { message = "Department renamed." }),
      RenameDepartmentResult.NotFound => NotFound(new { message = "Department not found." }),
      RenameDepartmentResult.DuplicateName => BadRequest(new { message = "Department name already exists." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>DELETE /api/admin/departments/{id}</summary>
  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new DeleteDepartmentCommand(id), cancellationToken);

    return result switch
    {
      DeleteDepartmentResult.Ok => NoContent(),
      DeleteDepartmentResult.NotFound => NotFound(new { message = "Department not found." }),
      DeleteDepartmentResult.HasEmployees => BadRequest(new { message = "Cannot delete a department with assigned employees." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>GET /api/admin/departments/{id}/required-skills</summary>
  [HttpGet("{id:guid}/required-skills")]
  public async Task<IActionResult> GetRequiredSkills(Guid id, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetRequiredSkillsQuery(id), cancellationToken);
    if (result is null) return NotFound(new { message = "Department not found." });
    return Ok(result);
  }

  public record AddRequiredSkillRequest(Guid SkillId);

  /// <summary>POST /api/admin/departments/{id}/required-skills</summary>
  [HttpPost("{id:guid}/required-skills")]
  public async Task<IActionResult> AddRequiredSkill(
      Guid id,
      [FromBody] AddRequiredSkillRequest request,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new AddRequiredSkillCommand(id, request.SkillId), cancellationToken);

    return result switch
    {
      AddRequiredSkillResult.Ok => Ok(new { message = "Required skill added." }),
      AddRequiredSkillResult.DepartmentNotFound => NotFound(new { message = "Department not found." }),
      AddRequiredSkillResult.SkillNotFound => NotFound(new { message = "Skill not found." }),
      AddRequiredSkillResult.AlreadyExists => Conflict(new { message = "Skill is already required for this department." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>DELETE /api/admin/departments/{id}/required-skills/{skillId}</summary>
  [HttpDelete("{id:guid}/required-skills/{skillId:guid}")]
  public async Task<IActionResult> RemoveRequiredSkill(
      Guid id, Guid skillId, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new RemoveRequiredSkillCommand(id, skillId), cancellationToken);

    return result switch
    {
      RemoveRequiredSkillResult.Ok => NoContent(),
      RemoveRequiredSkillResult.NotFound => NotFound(new { message = "Required skill assignment not found." }),
      _ => StatusCode(500),
    };
  }
}
