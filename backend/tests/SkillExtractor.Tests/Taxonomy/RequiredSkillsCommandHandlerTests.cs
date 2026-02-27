using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Application.Taxonomy.Commands.AddRequiredSkill;
using SkillExtractor.Application.Taxonomy.Commands.RemoveRequiredSkill;
using SkillExtractor.Application.Taxonomy.Queries.GetRequiredSkills;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Taxonomy;

public class GetRequiredSkillsQueryHandlerTests
{
  private readonly IDepartmentRepository _deptRepo = Substitute.For<IDepartmentRepository>();
  private readonly IDepartmentRequiredSkillRepository _reqSkillRepo = Substitute.For<IDepartmentRequiredSkillRepository>();

  private GetRequiredSkillsQueryHandler CreateHandler() => new(_deptRepo, _reqSkillRepo);

  [Fact]
  public async Task Returns_null_when_department_not_found()
  {
    var id = Guid.NewGuid();
    _deptRepo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Department?)null);

    var result = await CreateHandler().Handle(new GetRequiredSkillsQuery(id), default);

    result.Should().BeNull();
  }

  [Fact]
  public async Task Returns_skills_ordered_by_category_then_name()
  {
    var deptId = Guid.NewGuid();
    var dept = Department.Create("Engineering");
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>()).Returns(dept);

    var skillA = Skill.Create(Guid.NewGuid(), "Python", "Programming");
    var skillB = Skill.Create(Guid.NewGuid(), "Figma", "Design");
    var entries = new List<DepartmentRequiredSkill>
        {
            DepartmentRequiredSkill.Create("Engineering", skillA.Id),
            DepartmentRequiredSkill.Create("Engineering", skillB.Id),
        };
    // Assign nav props
    SetSkillNav(entries[0], skillA);
    SetSkillNav(entries[1], skillB);

    _reqSkillRepo.GetByDepartmentNameAsync("Engineering", Arg.Any<CancellationToken>()).Returns(entries);

    var result = await CreateHandler().Handle(new GetRequiredSkillsQuery(deptId), default);

    result.Should().NotBeNull();
    result![0].Name.Should().Be("Figma");  // Design comes before Programming
    result[1].Name.Should().Be("Python");
  }

  private static void SetSkillNav(DepartmentRequiredSkill entry, Skill skill)
  {
    typeof(DepartmentRequiredSkill)
        .GetProperty("Skill")!
        .SetValue(entry, skill);
    typeof(DepartmentRequiredSkill)
        .GetProperty("SkillId")!
        .SetValue(entry, skill.Id);
  }
}

public class AddRequiredSkillCommandHandlerTests
{
  private readonly IDepartmentRepository _deptRepo = Substitute.For<IDepartmentRepository>();
  private readonly ISkillRepository _skillRepo = Substitute.For<ISkillRepository>();
  private readonly IDepartmentRequiredSkillRepository _reqSkillRepo = Substitute.For<IDepartmentRequiredSkillRepository>();

  private AddRequiredSkillCommandHandler CreateHandler() => new(_deptRepo, _skillRepo, _reqSkillRepo);

  [Fact]
  public async Task Returns_DepartmentNotFound_when_department_missing()
  {
    var deptId = Guid.NewGuid();
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>()).Returns((Department?)null);

    var result = await CreateHandler().Handle(
        new AddRequiredSkillCommand(deptId, Guid.NewGuid()), default);

    result.Should().Be(AddRequiredSkillResult.DepartmentNotFound);
  }

  [Fact]
  public async Task Returns_SkillNotFound_when_skill_missing()
  {
    var deptId = Guid.NewGuid();
    var skillId = Guid.NewGuid();
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>())
        .Returns(Department.Create("Engineering"));
    _skillRepo.GetByIdAsync(skillId, Arg.Any<CancellationToken>())
        .Returns((Skill?)null);

    var result = await CreateHandler().Handle(
        new AddRequiredSkillCommand(deptId, skillId), default);

    result.Should().Be(AddRequiredSkillResult.SkillNotFound);
  }

  [Fact]
  public async Task Returns_AlreadyExists_when_mapping_exists()
  {
    var deptId = Guid.NewGuid();
    var skillId = Guid.NewGuid();
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>())
        .Returns(Department.Create("Engineering"));
    _skillRepo.GetByIdAsync(skillId, Arg.Any<CancellationToken>())
        .Returns(Skill.Create(skillId, "Python", "Programming"));
    _reqSkillRepo.ExistsAsync("Engineering", skillId, Arg.Any<CancellationToken>())
        .Returns(true);

    var result = await CreateHandler().Handle(
        new AddRequiredSkillCommand(deptId, skillId), default);

    result.Should().Be(AddRequiredSkillResult.AlreadyExists);
  }

  [Fact]
  public async Task Returns_Ok_and_saves_on_success()
  {
    var deptId = Guid.NewGuid();
    var skillId = Guid.NewGuid();
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>())
        .Returns(Department.Create("Engineering"));
    _skillRepo.GetByIdAsync(skillId, Arg.Any<CancellationToken>())
        .Returns(Skill.Create(skillId, "Python", "Programming"));
    _reqSkillRepo.ExistsAsync("Engineering", skillId, Arg.Any<CancellationToken>())
        .Returns(false);

    var result = await CreateHandler().Handle(
        new AddRequiredSkillCommand(deptId, skillId), default);

    result.Should().Be(AddRequiredSkillResult.Ok);
    await _reqSkillRepo.Received(1).AddAsync(Arg.Any<DepartmentRequiredSkill>(), Arg.Any<CancellationToken>());
    await _reqSkillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }
}

public class RemoveRequiredSkillCommandHandlerTests
{
  private readonly IDepartmentRepository _deptRepo = Substitute.For<IDepartmentRepository>();
  private readonly IDepartmentRequiredSkillRepository _reqSkillRepo = Substitute.For<IDepartmentRequiredSkillRepository>();

  private RemoveRequiredSkillCommandHandler CreateHandler() => new(_deptRepo, _reqSkillRepo);

  [Fact]
  public async Task Returns_NotFound_when_department_missing()
  {
    var deptId = Guid.NewGuid();
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>()).Returns((Department?)null);

    var result = await CreateHandler().Handle(
        new RemoveRequiredSkillCommand(deptId, Guid.NewGuid()), default);

    result.Should().Be(RemoveRequiredSkillResult.NotFound);
  }

  [Fact]
  public async Task Returns_NotFound_when_mapping_not_found()
  {
    var deptId = Guid.NewGuid();
    var skillId = Guid.NewGuid();
    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>())
        .Returns(Department.Create("Engineering"));
    _reqSkillRepo.FindAsync("Engineering", skillId, Arg.Any<CancellationToken>())
        .Returns((DepartmentRequiredSkill?)null);

    var result = await CreateHandler().Handle(
        new RemoveRequiredSkillCommand(deptId, skillId), default);

    result.Should().Be(RemoveRequiredSkillResult.NotFound);
  }

  [Fact]
  public async Task Returns_Ok_and_removes_on_success()
  {
    var deptId = Guid.NewGuid();
    var skillId = Guid.NewGuid();
    var entry = DepartmentRequiredSkill.Create("Engineering", skillId);

    _deptRepo.GetByIdAsync(deptId, Arg.Any<CancellationToken>())
        .Returns(Department.Create("Engineering"));
    _reqSkillRepo.FindAsync("Engineering", skillId, Arg.Any<CancellationToken>())
        .Returns(entry);

    var result = await CreateHandler().Handle(
        new RemoveRequiredSkillCommand(deptId, skillId), default);

    result.Should().Be(RemoveRequiredSkillResult.Ok);
    _reqSkillRepo.Received(1).Remove(entry);
    await _reqSkillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }
}
