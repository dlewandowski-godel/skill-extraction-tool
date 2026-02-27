using MediatR;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Application.EmployeeManagement.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string Role,
    Guid? DepartmentId) : IRequest<CreateEmployeeResult>;

public record CreateEmployeeResult(
    bool Succeeded,
    Guid? EmployeeId,
    string? TempPassword,
    string? Error);

public class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, CreateEmployeeResult>
{
  private readonly IUserManagementService _userMgmt;

  public CreateEmployeeCommandHandler(IUserManagementService userMgmt) => _userMgmt = userMgmt;

  public async Task<CreateEmployeeResult> Handle(
      CreateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var result = await _userMgmt.CreateUserAsync(
        request.FirstName,
        request.LastName,
        request.Email,
        request.Role,
        request.DepartmentId,
        cancellationToken);

    return new CreateEmployeeResult(result.Succeeded, result.UserId, result.TempPassword, result.Error);
  }
}
