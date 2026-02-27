using FluentAssertions;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Tests.DomainTests;

public class EmployeeSkillEntityTests
{
  private static readonly Guid SomeUserId = Guid.NewGuid();
  private static readonly Guid SomeSkillId = Guid.NewGuid();
  private static readonly Guid SomeDocId = Guid.NewGuid();

  // -----------------------------------------------------------------------
  // Creation guard-clause tests
  // -----------------------------------------------------------------------

  [Fact]
  public void Create_WithEmptyUserId_ThrowsArgumentException()
  {
    var act = () => EmployeeSkill.Create(
        Guid.Empty, SomeSkillId, ProficiencyLevel.Beginner, SomeDocId, DocumentType.CV);

    act.Should().Throw<ArgumentException>()
       .WithParameterName("userId");
  }

  [Fact]
  public void Create_WithEmptySkillId_ThrowsArgumentException()
  {
    var act = () => EmployeeSkill.Create(
        SomeUserId, Guid.Empty, ProficiencyLevel.Beginner, SomeDocId, DocumentType.CV);

    act.Should().Throw<ArgumentException>()
       .WithParameterName("skillId");
  }

  [Fact]
  public void Create_WithValidArgs_SetsAllProperties()
  {
    var skill = EmployeeSkill.Create(
        SomeUserId, SomeSkillId, ProficiencyLevel.Advanced, SomeDocId, DocumentType.IFU);

    skill.Id.Should().NotBe(Guid.Empty);
    skill.UserId.Should().Be(SomeUserId);
    skill.SkillId.Should().Be(SomeSkillId);
    skill.ProficiencyLevel.Should().Be(ProficiencyLevel.Advanced);
    skill.IsManualOverride.Should().BeFalse();
    skill.SourceDocumentId.Should().Be(SomeDocId);
    skill.SourceDocumentType.Should().Be(DocumentType.IFU);
    skill.ExtractedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
  }

  // -----------------------------------------------------------------------
  // SetManualOverrideProficiency
  // -----------------------------------------------------------------------

  [Fact]
  public void SetManualOverrideProficiency_SetsLevelAndMarksOverride()
  {
    var skill = EmployeeSkill.Create(
        SomeUserId, SomeSkillId, ProficiencyLevel.Beginner, SomeDocId, DocumentType.CV);

    skill.SetManualOverrideProficiency(ProficiencyLevel.Expert);

    skill.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
    skill.IsManualOverride.Should().BeTrue();
  }

  [Fact]
  public void SetManualOverrideProficiency_CanBeCancelledBySubsequentAdmin()
  {
    // Calling the method twice updates both times
    var skill = EmployeeSkill.Create(
        SomeUserId, SomeSkillId, ProficiencyLevel.Beginner, SomeDocId, DocumentType.CV);

    skill.SetManualOverrideProficiency(ProficiencyLevel.Advanced);
    skill.SetManualOverrideProficiency(ProficiencyLevel.Expert);

    skill.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
    skill.IsManualOverride.Should().BeTrue();
  }

  // -----------------------------------------------------------------------
  // UpdateAutoExtractedProficiency — does NOT flip IsManualOverride
  // -----------------------------------------------------------------------

  [Fact]
  public void UpdateAutoExtractedProficiency_UpdatesLevelWithoutSettingManualOverride()
  {
    var skill = EmployeeSkill.Create(
        SomeUserId, SomeSkillId, ProficiencyLevel.Intermediate, SomeDocId, DocumentType.CV);

    skill.UpdateAutoExtractedProficiency(ProficiencyLevel.Advanced);

    skill.ProficiencyLevel.Should().Be(ProficiencyLevel.Advanced);
    skill.IsManualOverride.Should().BeFalse("auto-extraction merge must not set IsManualOverride");
  }

  // -----------------------------------------------------------------------
  // Domain invariant: manual override vs auto-extraction
  // The domain invariant is enforced by the HANDLER (it checks IsManualOverride);
  // here we verify the entity has the flag available to be checked.
  // -----------------------------------------------------------------------

  [Fact]
  public void IsManualOverride_FalseAfterCreate_TrueAfterAdminSet()
  {
    var skill = EmployeeSkill.Create(
        SomeUserId, SomeSkillId, ProficiencyLevel.Beginner, SomeDocId, DocumentType.CV);

    skill.IsManualOverride.Should().BeFalse();

    skill.SetManualOverrideProficiency(ProficiencyLevel.Expert);
    skill.IsManualOverride.Should().BeTrue("admin override must lock the record");
  }
}
