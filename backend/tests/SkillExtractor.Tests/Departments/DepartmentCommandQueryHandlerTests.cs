using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Departments.Commands.CreateDepartment;
using SkillExtractor.Application.Departments.Commands.DeleteDepartment;
using SkillExtractor.Application.Departments.Queries.GetDepartments;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Tests.Departments;

public class GetDepartmentsQueryHandlerTests
{
  private readonly IDepartmentRepository _repo = Substitute.For<IDepartmentRepository>();
  private readonly GetDepartmentsQueryHandler _sut;

  public GetDepartmentsQueryHandlerTests()
      => _sut = new GetDepartmentsQueryHandler(_repo);

  [Fact]
  public async Task Handle_ReturnsDepartmentsWithCorrectEmployeeCounts()
  {
    var expected = new List<DepartmentSummaryDto>
        {
            new(Guid.NewGuid(), "Engineering", 5, 0),
            new(Guid.NewGuid(), "Marketing", 3, 0),
        };
    _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(expected);

    var result = await _sut.Handle(new GetDepartmentsQuery(), CancellationToken.None);

    result.Should().HaveCount(2);
    result[0].Name.Should().Be("Engineering");
    result[0].EmployeeCount.Should().Be(5);
  }

  [Fact]
  public async Task Handle_ReturnsEmptyList_WhenNoDepartments()
  {
    _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

    var result = await _sut.Handle(new GetDepartmentsQuery(), CancellationToken.None);

    result.Should().BeEmpty();
  }

  [Fact]
  public async Task Handle_DelegatesExactlyOnceToRepository()
  {
    _repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

    await _sut.Handle(new GetDepartmentsQuery(), CancellationToken.None);

    await _repo.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
  }
}

public class CreateDepartmentCommandHandlerTests
{
  private readonly IDepartmentRepository _repo = Substitute.For<IDepartmentRepository>();
  private readonly CreateDepartmentCommandHandler _sut;

  public CreateDepartmentCommandHandlerTests()
      => _sut = new CreateDepartmentCommandHandler(_repo);

  [Fact]
  public async Task Handle_UniqueName_CreatesDepartmentAndReturnsSuccess()
  {
    _repo.ExistsByNameAsync("Engineering", Arg.Any<CancellationToken>()).Returns(false);

    var result = await _sut.Handle(
        new CreateDepartmentCommand("Engineering"), CancellationToken.None);

    result.Succeeded.Should().BeTrue();
    result.DepartmentId.Should().NotBeNull();
    result.Error.Should().BeNull();
    await _repo.Received(1).AddAsync(Arg.Any<Department>(), Arg.Any<CancellationToken>());
    await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_DuplicateName_ReturnsConflictError()
  {
    _repo.ExistsByNameAsync("Engineering", Arg.Any<CancellationToken>()).Returns(true);

    var result = await _sut.Handle(
        new CreateDepartmentCommand("Engineering"), CancellationToken.None);

    result.Succeeded.Should().BeFalse();
    result.DepartmentId.Should().BeNull();
    result.Error.Should().Contain("already exists");
    await _repo.DidNotReceive().AddAsync(Arg.Any<Department>(), Arg.Any<CancellationToken>());
  }
}

public class DeleteDepartmentCommandHandlerTests
{
  private readonly IDepartmentRepository _repo = Substitute.For<IDepartmentRepository>();
  private readonly DeleteDepartmentCommandHandler _sut;
  private static readonly Guid DeptId = Guid.NewGuid();

  public DeleteDepartmentCommandHandlerTests()
      => _sut = new DeleteDepartmentCommandHandler(_repo);

  [Fact]
  public async Task Handle_ExistingDeptWithNoEmployees_DeletesSuccessfully()
  {
    _repo.ExistsByIdAsync(DeptId, Arg.Any<CancellationToken>()).Returns(true);
    _repo.HasEmployeesAsync(DeptId, Arg.Any<CancellationToken>()).Returns(false);

    var result = await _sut.Handle(
        new DeleteDepartmentCommand(DeptId), CancellationToken.None);

    result.Should().Be(DeleteDepartmentResult.Ok);
    await _repo.Received(1).DeleteAsync(DeptId, Arg.Any<CancellationToken>());
    await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_DepartmentHasEmployees_ReturnsHasEmployees()
  {
    _repo.ExistsByIdAsync(DeptId, Arg.Any<CancellationToken>()).Returns(true);
    _repo.HasEmployeesAsync(DeptId, Arg.Any<CancellationToken>()).Returns(true);

    var result = await _sut.Handle(
        new DeleteDepartmentCommand(DeptId), CancellationToken.None);

    result.Should().Be(DeleteDepartmentResult.HasEmployees);
    await _repo.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_NonExistentDepartment_ReturnsNotFound()
  {
    _repo.ExistsByIdAsync(DeptId, Arg.Any<CancellationToken>()).Returns(false);

    var result = await _sut.Handle(
        new DeleteDepartmentCommand(DeptId), CancellationToken.None);

    result.Should().Be(DeleteDepartmentResult.NotFound);
  }
}
