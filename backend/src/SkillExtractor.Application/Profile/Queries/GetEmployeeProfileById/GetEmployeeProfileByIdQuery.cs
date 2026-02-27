using MediatR;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Application.Profile.Queries.GetEmployeeProfileById;

public record GetEmployeeProfileByIdQuery(Guid EmployeeId) : IRequest<EmployeeProfileDto?>;

public class GetEmployeeProfileByIdQueryHandler : IRequestHandler<GetEmployeeProfileByIdQuery, EmployeeProfileDto?>
{
  private readonly IEmployeeSkillRepository _skillRepo;
  private readonly IUserRepository _userRepo;

  public GetEmployeeProfileByIdQueryHandler(
      IEmployeeSkillRepository skillRepo,
      IUserRepository userRepo)
  {
    _skillRepo = skillRepo;
    _userRepo = userRepo;
  }

  public async Task<EmployeeProfileDto?> Handle(
      GetEmployeeProfileByIdQuery request,
      CancellationToken cancellationToken)
  {
    var profile = await _userRepo.GetProfileInfoAsync(request.EmployeeId, cancellationToken);
    if (profile is null) return null;

    var employeeSkills = await _skillRepo.GetWithSkillsByUserAsync(request.EmployeeId, cancellationToken);

    var skillDtos = employeeSkills
        .Where(e => e.TaxonomySkill is not null)
        .Select(e => new SkillDto(
            SkillId: e.SkillId,
            SkillName: e.TaxonomySkill!.Name,
            Category: e.TaxonomySkill.Category,
            ProficiencyLevel: e.ProficiencyLevel.ToString(),
            IsManualOverride: e.IsManualOverride,
            ExtractedAt: e.ExtractedAt))
        .OrderBy(s => s.Category)
        .ThenBy(s => s.SkillName)
        .ToList();

    return new EmployeeProfileDto(request.EmployeeId, profile.FullName, profile.FirstName, profile.LastName, profile.Department, profile.DepartmentId, profile.Role, profile.IsActive, skillDtos);
  }
}
