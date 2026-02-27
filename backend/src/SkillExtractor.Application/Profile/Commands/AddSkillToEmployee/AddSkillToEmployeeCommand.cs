using MediatR;
using Microsoft.Extensions.Logging;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Application.Profile.Commands.AddSkillToEmployee;

public record AddSkillToEmployeeCommand(
    Guid AdminId,
    Guid EmployeeId,
    Guid SkillId,
    ProficiencyLevel ProficiencyLevel) : IRequest<AddSkillResult>;

public enum AddSkillResult { Ok, SkillNotFound }

public class AddSkillToEmployeeCommandHandler : IRequestHandler<AddSkillToEmployeeCommand, AddSkillResult>
{
  private readonly IEmployeeSkillRepository _empSkillRepo;
  private readonly ISkillRepository _skillRepo;
  private readonly ILogger<AddSkillToEmployeeCommandHandler> _logger;

  public AddSkillToEmployeeCommandHandler(
      IEmployeeSkillRepository empSkillRepo,
      ISkillRepository skillRepo,
      ILogger<AddSkillToEmployeeCommandHandler> logger)
  {
    _empSkillRepo = empSkillRepo;
    _skillRepo = skillRepo;
    _logger = logger;
  }

  public async Task<AddSkillResult> Handle(
      AddSkillToEmployeeCommand request,
      CancellationToken cancellationToken)
  {
    var skill = await _skillRepo.GetByIdAsync(request.SkillId, cancellationToken);
    if (skill is null) return AddSkillResult.SkillNotFound;

    var existing = await _empSkillRepo.GetByUserAndSkillAsync(
        request.EmployeeId, request.SkillId, cancellationToken);

    if (existing is not null)
    {
      // Upsert — update existing record and lock it as manual
      existing.SetManualOverrideProficiency(request.ProficiencyLevel);
    }
    else
    {
      var newSkill = EmployeeSkill.Create(
          request.EmployeeId,
          request.SkillId,
          request.ProficiencyLevel,
          sourceDocumentId: Guid.Empty,       // no document — manual entry
          sourceDocumentType: DocumentType.CV);
      newSkill.SetManualOverrideProficiency(request.ProficiencyLevel);
      await _empSkillRepo.AddAsync(newSkill, cancellationToken);
    }

    await _empSkillRepo.SaveChangesAsync(cancellationToken);

    _logger.LogInformation(
        "[AuditLog] Admin {AdminId} added skill {SkillId} to employee {EmployeeId}",
        request.AdminId, request.SkillId, request.EmployeeId);

    return AddSkillResult.Ok;
  }
}
