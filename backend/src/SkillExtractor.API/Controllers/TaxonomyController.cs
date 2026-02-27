using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Taxonomy.Commands.ActivateSkill;
using SkillExtractor.Application.Taxonomy.Commands.AddSkill;
using SkillExtractor.Application.Taxonomy.Commands.DeactivateSkill;
using SkillExtractor.Application.Taxonomy.Commands.UpdateSkill;
using SkillExtractor.Application.Taxonomy.Queries.GetTaxonomy;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/admin/taxonomy")]
[Authorize(Roles = "Admin")]
public class TaxonomyController : ControllerBase
{
  private readonly IMediator _mediator;

  public TaxonomyController(IMediator mediator) => _mediator = mediator;

  /// <summary>GET /api/admin/taxonomy?search=&amp;category=</summary>
  [HttpGet]
  public async Task<IActionResult> GetAll(
      [FromQuery] string? search,
      [FromQuery] string? category,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetTaxonomyQuery(search, category), cancellationToken);
    return Ok(result);
  }

  public record AddSkillRequest(string Name, string Category, List<string> Aliases);

  /// <summary>POST /api/admin/taxonomy</summary>
  [HttpPost]
  public async Task<IActionResult> Add(
      [FromBody] AddSkillRequest request,
      CancellationToken cancellationToken)
  {
    var (result, skillId) = await _mediator.Send(
        new AddSkillCommand(request.Name, request.Category, request.Aliases), cancellationToken);

    return result switch
    {
      AddSkillResult.Ok => Ok(new { skillId }),
      AddSkillResult.Conflict => Conflict(new { message = "A skill with this name already exists in the category." }),
      _ => StatusCode(500),
    };
  }

  public record UpdateSkillRequest(string Name, string Category, List<string> Aliases);

  /// <summary>PUT /api/admin/taxonomy/{id}</summary>
  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(
      Guid id,
      [FromBody] UpdateSkillRequest request,
      CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
        new UpdateSkillCommand(id, request.Name, request.Category, request.Aliases), cancellationToken);

    return result switch
    {
      UpdateSkillResult.Ok => Ok(new { message = "Skill updated." }),
      UpdateSkillResult.NotFound => NotFound(new { message = "Skill not found." }),
      UpdateSkillResult.Conflict => Conflict(new { message = "A skill with this name already exists in the category." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>DELETE /api/admin/taxonomy/{id} — deactivates the skill</summary>
  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeactivateSkillCommand(id), cancellationToken);

    return result switch
    {
      DeactivateSkillResult.Ok => NoContent(),
      DeactivateSkillResult.NotFound => NotFound(new { message = "Skill not found." }),
      _ => StatusCode(500),
    };
  }

  /// <summary>PUT /api/admin/taxonomy/{id}/activate</summary>
  [HttpPut("{id:guid}/activate")]
  public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ActivateSkillCommand(id), cancellationToken);

    return result switch
    {
      ActivateSkillResult.Ok => Ok(new { message = "Skill activated." }),
      ActivateSkillResult.NotFound => NotFound(new { message = "Skill not found." }),
      _ => StatusCode(500),
    };
  }
}
