using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Common;
using SkillExtractor.Application.EmployeeManagement.Queries.GetEmployees;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Tests.EmployeeManagement;

public class GetEmployeesQueryHandlerTests
{
  private readonly IEmployeeRepository _repo = Substitute.For<IEmployeeRepository>();
  private readonly GetEmployeesQueryHandler _sut;

  public GetEmployeesQueryHandlerTests()
      => _sut = new GetEmployeesQueryHandler(_repo);

  private static EmployeeListItemDto MakeEmployee(bool isActive = true, string? dept = "Engineering") =>
      new(Guid.NewGuid(), "Jane", "Doe", "jane@test.com", dept, "User", isActive, null);

  [Fact]
  public async Task Handle_ReturnsPagedResult_RespectingPageAndPageSize()
  {
    var items = Enumerable.Range(0, 5).Select(_ => MakeEmployee()).ToList();
    var paged = new PagedResult<EmployeeListItemDto>(items, 50, 1, 5);

    _repo.GetPagedAsync(null, null, 1, 5, Arg.Any<CancellationToken>()).Returns(paged);

    var result = await _sut.Handle(
        new GetEmployeesQuery(null, null, 1, 5), CancellationToken.None);

    result.TotalCount.Should().Be(50);
    result.Items.Should().HaveCount(5);
    result.Page.Should().Be(1);
    result.PageSize.Should().Be(5);
  }

  [Fact]
  public async Task Handle_SearchFilter_IsPassedToRepository()
  {
    var paged = new PagedResult<EmployeeListItemDto>([], 0, 1, 20);
    _repo.GetPagedAsync("john", null, 1, 20, Arg.Any<CancellationToken>()).Returns(paged);

    await _sut.Handle(new GetEmployeesQuery("john", null, 1, 20), CancellationToken.None);

    await _repo.Received(1).GetPagedAsync(
        "john", null, 1, 20, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_DepartmentFilter_IsPassedToRepository()
  {
    var items = new List<EmployeeListItemDto> { MakeEmployee(dept: "Engineering") };
    var paged = new PagedResult<EmployeeListItemDto>(items, 1, 1, 20);

    _repo.GetPagedAsync(null, "Engineering", 1, 20, Arg.Any<CancellationToken>())
        .Returns(paged);

    var result = await _sut.Handle(
        new GetEmployeesQuery(null, "Engineering", 1, 20), CancellationToken.None);

    result.Items.Should().OnlyContain(e => e.DepartmentName == "Engineering");
  }

  [Fact]
  public async Task Handle_TotalCountReflectsFullUnpaginatedResult()
  {
    var items = Enumerable.Range(0, 20).Select(_ => MakeEmployee()).ToList();
    var paged = new PagedResult<EmployeeListItemDto>(items, 87, 1, 20);
    _repo.GetPagedAsync(null, null, 1, 20, Arg.Any<CancellationToken>()).Returns(paged);

    var result = await _sut.Handle(
        new GetEmployeesQuery(null, null, 1, 20), CancellationToken.None);

    result.TotalCount.Should().Be(87);
    result.Items.Should().HaveCount(20);
  }

  [Fact]
  public async Task Handle_DeactivatedEmployees_AreIncludedInList()
  {
    var items = new List<EmployeeListItemDto>
        {
            MakeEmployee(isActive: true),
            MakeEmployee(isActive: false),
        };
    var paged = new PagedResult<EmployeeListItemDto>(items, 2, 1, 20);
    _repo.GetPagedAsync(null, null, 1, 20, Arg.Any<CancellationToken>()).Returns(paged);

    var result = await _sut.Handle(
        new GetEmployeesQuery(null, null, 1, 20), CancellationToken.None);

    result.Items.Should().Contain(e => !e.IsActive);
    result.Items.Should().Contain(e => e.IsActive);
  }
}
