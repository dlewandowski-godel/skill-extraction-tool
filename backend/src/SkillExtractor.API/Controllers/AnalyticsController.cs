using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Analytics.Queries.GetProficiencyDistribution;
using SkillExtractor.Application.Analytics.Queries.GetSkillGaps;
using SkillExtractor.Application.Analytics.Queries.GetSkillsByDepartment;
using SkillExtractor.Application.Analytics.Queries.GetTopSkills;
using SkillExtractor.Application.Analytics.Queries.GetUploadActivity;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/admin/analytics")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
  private readonly IMediator _mediator;

  public AnalyticsController(IMediator mediator) => _mediator = mediator;

  /// <summary>GET /api/admin/analytics/top-skills?limit=10</summary>
  [HttpGet("top-skills")]
  public async Task<IActionResult> GetTopSkills(
      [FromQuery] int limit = 10, CancellationToken ct = default)
      => Ok(await _mediator.Send(new GetTopSkillsQuery(limit), ct));

  /// <summary>GET /api/admin/analytics/skills-by-department</summary>
  [HttpGet("skills-by-department")]
  public async Task<IActionResult> GetSkillsByDepartment(CancellationToken ct = default)
      => Ok(await _mediator.Send(new GetSkillsByDepartmentQuery(), ct));

  /// <summary>GET /api/admin/analytics/skill-gaps?department=Engineering</summary>
  [HttpGet("skill-gaps")]
  public async Task<IActionResult> GetSkillGaps(
      [FromQuery] string? department = null, CancellationToken ct = default)
      => Ok(await _mediator.Send(new GetSkillGapsQuery(department), ct));

  /// <summary>GET /api/admin/analytics/upload-activity?period=30d</summary>
  [HttpGet("upload-activity")]
  public async Task<IActionResult> GetUploadActivity(
      [FromQuery] string period = "30d", CancellationToken ct = default)
  {
    var days = period switch { "7d" => 7, "90d" => 90, _ => 30 };
    return Ok(await _mediator.Send(new GetUploadActivityQuery(days), ct));
  }

  /// <summary>GET /api/admin/analytics/proficiency-distribution</summary>
  [HttpGet("proficiency-distribution")]
  public async Task<IActionResult> GetProficiencyDistribution(CancellationToken ct = default)
      => Ok(await _mediator.Send(new GetProficiencyDistributionQuery(), ct));
}
