using MediatR;
using Microsoft.Extensions.Logging;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Application.Profile.Commands.ChangeProficiency;

public record ChangeProficiencyCommand(
    Guid AdminId,
    Guid EmployeeId,
    Guid SkillId,
    ProficiencyLevel ProficiencyLevel) : IRequest<ChangeProficiencyResult>;

public enum ChangeProficiencyResult { Ok, NotFound }

public class ChangeProficiencyCommandHandler : IRequestHandler<ChangeProficiencyCommand, ChangeProficiencyResult>
{
  private readonly IEmployeeSkillRepository _empSkillRepo;
  private readonly ILogger<ChangeProficiencyCommandHandler> _logger;

  public ChangeProficiencyCommandHandler(
      IEmployeeSkillRepository empSkillRepo,
      ILogger<ChangeProficiencyCommandHandler> logger)
  {
    _empSkillRepo = empSkillRepo;
    _logger = logger;
  }

  public async Task<ChangeProficiencyResult> Handle(
      ChangeProficiencyCommand request,
      CancellationToken cancellationToken)
  {
    var existing = await _empSkillRepo.GetByUserAndSkillAsync(
        request.EmployeeId, request.SkillId, cancellationToken);

    if (existing is null) return ChangeProficiencyResult.NotFound;

    existing.SetManualOverrideProficiency(request.ProficiencyLevel);
    await _empSkillRepo.SaveChangesAsync(cancellationToken);

    _logger.LogInformation(
        "[AuditLog] Admin {AdminId} changed proficiency of skill {SkillId} for employee {EmployeeId} to {Level}",
        request.AdminId, request.SkillId, request.EmployeeId, request.ProficiencyLevel);

    return ChangeProficiencyResult.Ok;
  }
}
