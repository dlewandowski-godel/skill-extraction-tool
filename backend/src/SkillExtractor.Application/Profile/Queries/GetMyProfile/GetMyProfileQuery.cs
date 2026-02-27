using MediatR;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Application.Profile.Queries.GetMyProfile;

public record GetMyProfileQuery(Guid UserId) : IRequest<EmployeeProfileDto>;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, EmployeeProfileDto>
{
  private readonly IEmployeeSkillRepository _skillRepo;
  private readonly IUserRepository _userRepo;

  public GetMyProfileQueryHandler(
      IEmployeeSkillRepository skillRepo,
      IUserRepository userRepo)
  {
    _skillRepo = skillRepo;
    _userRepo = userRepo;
  }

  public async Task<EmployeeProfileDto> Handle(
      GetMyProfileQuery request,
      CancellationToken cancellationToken)
  {
    var profile = await _userRepo.GetProfileInfoAsync(request.UserId, cancellationToken);
    var fullName = profile?.FullName ?? string.Empty;

    var employeeSkills = await _skillRepo.GetWithSkillsByUserAsync(request.UserId, cancellationToken);

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

    return new EmployeeProfileDto(request.UserId, fullName, profile?.FirstName ?? string.Empty, profile?.LastName ?? string.Empty, profile?.Department, profile?.DepartmentId, profile?.Role, profile?.IsActive ?? true, skillDtos);
  }
}
