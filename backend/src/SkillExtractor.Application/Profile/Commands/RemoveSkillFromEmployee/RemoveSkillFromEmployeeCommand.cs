using MediatR;
using Microsoft.Extensions.Logging;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Application.Profile.Commands.RemoveSkillFromEmployee;

public record RemoveSkillFromEmployeeCommand(
    Guid AdminId,
    Guid EmployeeId,
    Guid SkillId) : IRequest<RemoveSkillResult>;

public enum RemoveSkillResult { Ok, NotFound }

public class RemoveSkillFromEmployeeCommandHandler : IRequestHandler<RemoveSkillFromEmployeeCommand, RemoveSkillResult>
{
  private readonly IEmployeeSkillRepository _empSkillRepo;
  private readonly ILogger<RemoveSkillFromEmployeeCommandHandler> _logger;

  public RemoveSkillFromEmployeeCommandHandler(
      IEmployeeSkillRepository empSkillRepo,
      ILogger<RemoveSkillFromEmployeeCommandHandler> logger)
  {
    _empSkillRepo = empSkillRepo;
    _logger = logger;
  }

  public async Task<RemoveSkillResult> Handle(
      RemoveSkillFromEmployeeCommand request,
      CancellationToken cancellationToken)
  {
    var existing = await _empSkillRepo.GetByUserAndSkillAsync(
        request.EmployeeId, request.SkillId, cancellationToken);

    if (existing is null) return RemoveSkillResult.NotFound;

    _empSkillRepo.Remove(existing);
    await _empSkillRepo.SaveChangesAsync(cancellationToken);

    _logger.LogInformation(
        "[AuditLog] Admin {AdminId} removed skill {SkillId} from employee {EmployeeId}",
        request.AdminId, request.SkillId, request.EmployeeId);

    return RemoveSkillResult.Ok;
  }
}
