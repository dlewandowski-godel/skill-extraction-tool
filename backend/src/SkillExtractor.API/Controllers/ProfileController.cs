using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Profile.Queries.GetMyProfile;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
  private readonly IMediator _mediator;

  public ProfileController(IMediator mediator) => _mediator = mediator;

  /// <summary>GET /api/profile/me — returns the authenticated user's skill profile.</summary>
  [HttpGet("me")]
  public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
  {
    var userId = GetUserId();
    if (userId is null) return Unauthorized();

    var profile = await _mediator.Send(new GetMyProfileQuery(userId.Value), cancellationToken);
    return Ok(profile);
  }

  private Guid? GetUserId()
  {
    var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");
    return Guid.TryParse(value, out var id) ? id : null;
  }
}
