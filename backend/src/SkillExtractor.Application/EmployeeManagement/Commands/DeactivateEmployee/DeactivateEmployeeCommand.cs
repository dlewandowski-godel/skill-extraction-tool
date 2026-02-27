using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.EmployeeManagement.Commands.DeactivateEmployee;

public record DeactivateEmployeeCommand(Guid EmployeeId, Guid AdminId)
    : IRequest<DeactivateEmployeeResult>;

public enum DeactivateEmployeeResult
{
  Ok,
  NotFound,
  CannotDeactivateSelf,
}

public class DeactivateEmployeeCommandHandler
    : IRequestHandler<DeactivateEmployeeCommand, DeactivateEmployeeResult>
{
  private readonly IUserManagementService _userMgmt;

  public DeactivateEmployeeCommandHandler(IUserManagementService userMgmt) => _userMgmt = userMgmt;

  public async Task<DeactivateEmployeeResult> Handle(
      DeactivateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var result = await _userMgmt.DeactivateUserAsync(
        request.EmployeeId, request.AdminId, cancellationToken);

    if (!result.Succeeded)
    {
      return result.Error switch
      {
        "NotFound" => DeactivateEmployeeResult.NotFound,
        "CannotDeactivateSelf" => DeactivateEmployeeResult.CannotDeactivateSelf,
        _ => DeactivateEmployeeResult.NotFound,
      };
    }

    return DeactivateEmployeeResult.Ok;
  }
}
