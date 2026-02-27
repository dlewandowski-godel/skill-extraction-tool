using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.EmployeeManagement.Commands.EditEmployee;

public record EditEmployeeCommand(
    Guid EmployeeId,
    Guid CallerId,
    string FirstName,
    string LastName,
    Guid? DepartmentId,
    string Role) : IRequest<EditEmployeeResult>;

public enum EditEmployeeResult
{
  Ok,
  NotFound,
  CannotChangeOwnRole,
  Error,
}

public class EditEmployeeCommandHandler
    : IRequestHandler<EditEmployeeCommand, EditEmployeeResult>
{
  private readonly IUserManagementService _userMgmt;

  public EditEmployeeCommandHandler(IUserManagementService userMgmt) => _userMgmt = userMgmt;

  public async Task<EditEmployeeResult> Handle(
      EditEmployeeCommand request, CancellationToken cancellationToken)
  {
    var result = await _userMgmt.UpdateUserAsync(
        request.EmployeeId,
        request.FirstName,
        request.LastName,
        request.DepartmentId,
        request.Role,
        request.CallerId,
        cancellationToken);

    if (!result.Succeeded)
    {
      return result.Error switch
      {
        "NotFound" => EditEmployeeResult.NotFound,
        "CannotChangeOwnRole" => EditEmployeeResult.CannotChangeOwnRole,
        _ => EditEmployeeResult.Error,
      };
    }

    return EditEmployeeResult.Ok;
  }
}
