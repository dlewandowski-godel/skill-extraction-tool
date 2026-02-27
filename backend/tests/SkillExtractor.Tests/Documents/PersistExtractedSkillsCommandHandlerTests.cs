using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SkillExtractor.Application.Documents.Commands.ProcessDocument;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Tests.Documents;

/// <summary>
/// Focused tests for the persistence behaviour of ProcessDocumentCommandHandler (US-4.5):
/// - replaces auto-extracted skills by document type
/// - never removes manual overrides
/// - merges CV and IFU extractions with highest proficiency winning
/// - persists all skills in a single SaveChangesAsync call
/// </summary>
public class PersistExtractedSkillsCommandHandlerTests
{
  private readonly IDocumentRepository _documentRepo = Substitute.For<IDocumentRepository>();
  private readonly IEmployeeSkillRepository _skillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly IPdfTextExtractor _pdfExtractor = Substitute.For<IPdfTextExtractor>();
  private readonly ISkillExtractor _skillExtractor = Substitute.For<ISkillExtractor>();
  private readonly IProficiencyInferenceService _proficiency = Substitute.For<IProficiencyInferenceService>();

  private readonly ProcessDocumentCommandHandler _sut;

  private static readonly Guid UserId = Guid.NewGuid();
  private static readonly Guid SkillAId = Guid.NewGuid();  // e.g. "Python"
  private static readonly Guid SkillBId = Guid.NewGuid();  // e.g. "Machine Learning"
  private static readonly Guid DocCV = Guid.NewGuid();
  private static readonly Guid DocIFU = Guid.NewGuid();

  public PersistExtractedSkillsCommandHandlerTests()
  {
    _sut = new ProcessDocumentCommandHandler(
        _documentRepo,
        _skillRepo,
        _pdfExtractor,
        _skillExtractor,
        _proficiency,
        NullLogger<ProcessDocumentCommandHandler>.Instance);

    // Default: proficiency inference returns its input unchanged
    _proficiency
        .InferProficiency(Arg.Any<string>(), Arg.Any<IReadOnlyList<ExtractedSkill>>())
        .Returns(args => (IReadOnlyList<ExtractedSkill>)args[1]);

    // Default: no existing skills for user
    _skillRepo
        .GetByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill>());
  }

  // -----------------------------------------------------------------------
  // Helpers
  // -----------------------------------------------------------------------

  private Document CreateAndSetupDocument(DocumentType type)
  {
    var doc = Document.Create(UserId, "file.pdf", "/uploads/file.pdf", type);
    _documentRepo.GetByIdAsync(doc.Id, Arg.Any<CancellationToken>()).Returns(doc);
    _pdfExtractor.ExtractText(Arg.Any<string>()).Returns("some text");
    return doc;
  }

  // -----------------------------------------------------------------------
  // Tests
  // -----------------------------------------------------------------------

  [Fact]
  public async Task Handle_AutoExtractedSkillsFromSameDocType_AreDeletedBeforeInserting()
  {
    // Arrange: process a CV document
    var doc = CreateAndSetupDocument(DocumentType.CV);
    _skillExtractor.ExtractSkills(Arg.Any<string>()).Returns(Array.Empty<ExtractedSkill>());

    // Act
    await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert: DeleteAutoExtractedByDocumentTypeAsync called for CV before AddRange
    await _skillRepo.Received(1).DeleteAutoExtractedByDocumentTypeAsync(
        UserId, DocumentType.CV, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ManualOverrides_AreNotIncludedInDeletion()
  {
    // Arrange: user has a manual override for SkillA
    var manualOverride = EmployeeSkill.Create(
        UserId, SkillAId, ProficiencyLevel.Expert, DocCV, DocumentType.CV);
    manualOverride.SetManualOverrideProficiency(ProficiencyLevel.Expert); // IsManualOverride = true

    // GetByUserAsync returns that override after deletion of auto-extracted skills
    _skillRepo
        .GetByUserAsync(UserId, Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill> { manualOverride });

    var doc = CreateAndSetupDocument(DocumentType.CV);

    // The newly extracted SkillA at Beginner should not overwrite the manual Expert
    _skillExtractor.ExtractSkills(Arg.Any<string>())
        .Returns(new[] { new ExtractedSkill(SkillAId, "Python", "Programming", "Python", 1, ProficiencyLevel.Beginner) });

    // Act
    await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert: AddRange should not contain SkillA (manual override excluded from merge)
    _skillRepo.Received(1).AddRange(
        Arg.Is<IEnumerable<EmployeeSkill>>(skills =>
            !skills.Any(s => s.SkillId == SkillAId)));
  }

  [Fact]
  public async Task Handle_SameSkillExtractedByBothDocTypes_HigherProficiencyWins()
  {
    // Arrange: user already has Python from CV at Intermediate
    var existingCvSkill = EmployeeSkill.Create(
        UserId, SkillAId, ProficiencyLevel.Intermediate, DocCV, DocumentType.CV);

    _skillRepo
        .GetByUserAsync(UserId, Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill> { existingCvSkill });

    // IFU document extracts Python at Advanced (higher)
    var ifuDoc = CreateAndSetupDocument(DocumentType.IFU);
    _skillExtractor.ExtractSkills(Arg.Any<string>())
        .Returns(new[]
        {
                new ExtractedSkill(SkillAId, "Python", "Programming", "Python", 1, ProficiencyLevel.Advanced)
        });

    // Act
    await _sut.Handle(new ProcessDocumentCommand(ifuDoc.Id), CancellationToken.None);

    // Assert: existing skill updated to Advanced, not duplicated
    existingCvSkill.ProficiencyLevel.Should().Be(ProficiencyLevel.Advanced);
    existingCvSkill.IsManualOverride.Should().BeFalse("auto-merge must not flag IsManualOverride");

    // Should NOT insert a new row for SkillA (merge, not insert)
    _skillRepo.Received(1).AddRange(
        Arg.Is<IEnumerable<EmployeeSkill>>(skills => !skills.Any(s => s.SkillId == SkillAId)));
  }

  [Fact]
  public async Task Handle_SameSkillExtractedByBothDocTypes_LowerProficiencyNotDowngraded()
  {
    // Arrange: user already has Python from CV at Expert
    var existingCvSkill = EmployeeSkill.Create(
        UserId, SkillAId, ProficiencyLevel.Expert, DocCV, DocumentType.CV);

    _skillRepo
        .GetByUserAsync(UserId, Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill> { existingCvSkill });

    // IFU extracts the same skill at a lower level (Beginner)
    var ifuDoc = CreateAndSetupDocument(DocumentType.IFU);
    _skillExtractor.ExtractSkills(Arg.Any<string>())
        .Returns(new[]
        {
                new ExtractedSkill(SkillAId, "Python", "Programming", "Python", 1, ProficiencyLevel.Beginner)
        });

    // Act
    await _sut.Handle(new ProcessDocumentCommand(ifuDoc.Id), CancellationToken.None);

    // Assert: existing Expert level is preserved
    existingCvSkill.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert,
        "lower proficiency must not downgrade existing record");
  }

  [Fact]
  public async Task Handle_AllNewSkills_SavedInSingleSaveChangesCall()
  {
    // Arrange: multiple skills extracted, no existing records
    var doc = CreateAndSetupDocument(DocumentType.CV);
    _skillExtractor.ExtractSkills(Arg.Any<string>())
        .Returns(new[]
        {
                new ExtractedSkill(SkillAId, "Python",           "Programming", "Python",           1),
                new ExtractedSkill(SkillBId, "Machine Learning", "ML & AI",     "machine learning", 2),
        });

    // Act
    await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert: skill repository SaveChangesAsync called exactly once for persistence
    await _skillRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }
}
