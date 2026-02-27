using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.EmployeeManagement.Commands.ActivateEmployee;

public record ActivateEmployeeCommand(Guid EmployeeId) : IRequest<ActivateEmployeeResult>;

public enum ActivateEmployeeResult { Ok, NotFound }

public class ActivateEmployeeCommandHandler
    : IRequestHandler<ActivateEmployeeCommand, ActivateEmployeeResult>
{
  private readonly IUserManagementService _userMgmt;

  public ActivateEmployeeCommandHandler(IUserManagementService userMgmt) => _userMgmt = userMgmt;

  public async Task<ActivateEmployeeResult> Handle(
      ActivateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var result = await _userMgmt.ActivateUserAsync(request.EmployeeId, cancellationToken);
    return result.Succeeded ? ActivateEmployeeResult.Ok : ActivateEmployeeResult.NotFound;
  }
}
