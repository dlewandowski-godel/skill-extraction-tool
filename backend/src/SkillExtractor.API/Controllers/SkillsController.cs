using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Taxonomy.Queries.GetAllSkills;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/skills")]
[Authorize]
public class SkillsController : ControllerBase
{
  private readonly IMediator _mediator;

  public SkillsController(IMediator mediator) => _mediator = mediator;

  /// <summary>
  /// Returns all active taxonomy skills, ordered by category then name.
  /// Used by the admin "Add Skill" dialog.
  /// </summary>
  [HttpGet]
  public async Task<IActionResult> GetAll(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetAllSkillsQuery(), ct);
    return Ok(result);
  }
}
