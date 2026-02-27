using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Application.Documents.Commands.UploadDocument;
using SkillExtractor.Application.Documents.Queries.GetMyDocuments;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.API.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
  private readonly IMediator _mediator;

  public DocumentsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  /// <summary>POST /api/documents/upload</summary>
  [HttpPost("upload")]
  [RequestSizeLimit(10 * 1024 * 1024 + 65536)] // 10 MB + headers buffer
  public async Task<IActionResult> Upload(
      IFormFile file,
      [FromForm] string documentType,
      CancellationToken cancellationToken)
  {
    var userId = GetUserId();
    if (userId is null)
      return Unauthorized();

    if (!Enum.TryParse<DocumentType>(documentType, ignoreCase: true, out var parsedType))
      return BadRequest(new { message = $"Invalid documentType '{documentType}'. Allowed values: CV, IFU." });

    var command = new UploadDocumentCommand(
        UserId: userId.Value,
        FileName: file.FileName,
        ContentType: file.ContentType,
        FileSize: file.Length,
        FileContent: file.OpenReadStream(),
        DocumentType: parsedType);

    try
    {
      var result = await _mediator.Send(command, cancellationToken);
      return Ok(new { documentId = result.DocumentId, status = result.Status });
    }
    catch (ValidationException ex)
    {
      var errors = ex.Errors.Select(e => e.ErrorMessage).Distinct();
      return BadRequest(new { message = string.Join(" ", errors) });
    }
  }

  /// <summary>GET /api/documents/my</summary>
  [HttpGet("my")]
  public async Task<IActionResult> GetMyDocuments(CancellationToken cancellationToken)
  {
    var userId = GetUserId();
    if (userId is null)
      return Unauthorized();

    var documents = await _mediator.Send(new GetMyDocumentsQuery(userId.Value), cancellationToken);
    return Ok(documents);
  }

  private Guid? GetUserId()
  {
    var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");
    return Guid.TryParse(value, out var id) ? id : null;
  }
}
