using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SkillExtractor.Application.Documents.Commands.ProcessDocument;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Domain.Interfaces;

namespace SkillExtractor.Tests.Documents;

public class ProcessDocumentCommandHandlerTests
{
  // -----------------------------------------------------------------------
  // Mocks
  // -----------------------------------------------------------------------
  private readonly IDocumentRepository _documentRepo = Substitute.For<IDocumentRepository>();
  private readonly IEmployeeSkillRepository _skillRepo = Substitute.For<IEmployeeSkillRepository>();
  private readonly IPdfTextExtractor _pdfExtractor = Substitute.For<IPdfTextExtractor>();
  private readonly ISkillExtractor _skillExtractor = Substitute.For<ISkillExtractor>();
  private readonly IProficiencyInferenceService _proficiency = Substitute.For<IProficiencyInferenceService>();

  private readonly ProcessDocumentCommandHandler _sut;

  private static readonly Guid UserId = Guid.NewGuid();
  private static readonly Guid DocumentId = Guid.NewGuid();
  private static readonly Guid SkillId = Guid.NewGuid();

  public ProcessDocumentCommandHandlerTests()
  {
    _sut = new ProcessDocumentCommandHandler(
        _documentRepo,
        _skillRepo,
        _pdfExtractor,
        _skillExtractor,
        _proficiency,
        NullLogger<ProcessDocumentCommandHandler>.Instance);

    // Default: proficiency inference returns the input unchanged
    _proficiency
        .InferProficiency(Arg.Any<string>(), Arg.Any<IReadOnlyList<ExtractedSkill>>())
        .Returns(args => (IReadOnlyList<ExtractedSkill>)args[1]);

    // Default: no existing skills for the user
    _skillRepo
        .GetByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(new List<EmployeeSkill>());
  }

  // -----------------------------------------------------------------------
  // Helpers
  // -----------------------------------------------------------------------
  private Document CreateDocument(DocumentType type = DocumentType.CV)
  {
    var doc = Document.Create(UserId, "cv.pdf", "/uploads/cv.pdf", type);
    return doc;
  }

  private void SetupDocument(Document document)
  {
    _documentRepo.GetByIdAsync(document.Id, Arg.Any<CancellationToken>())
                 .Returns(document);
  }

  // -----------------------------------------------------------------------
  // Tests
  // -----------------------------------------------------------------------

  [Fact]
  public async Task Handle_HappyPath_StatusTransitionsPendingProcessingDone()
  {
    // Arrange
    var doc = CreateDocument();
    SetupDocument(doc);
    _pdfExtractor.ExtractText(Arg.Any<string>()).Returns("Python developer");
    _skillExtractor.ExtractSkills(Arg.Any<string>())
                   .Returns(new[] { new ExtractedSkill(SkillId, "Python", "Programming", "Python", 1) });

    // Act
    await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert
    doc.Status.Should().Be(DocumentStatus.Done);
  }

  [Fact]
  public async Task Handle_HappyPath_CallsExtractTextWithDocumentFilePath()
  {
    // Arrange
    var doc = CreateDocument();
    SetupDocument(doc);
    _pdfExtractor.ExtractText(Arg.Any<string>()).Returns("some text");
    _skillExtractor.ExtractSkills(Arg.Any<string>()).Returns(Array.Empty<ExtractedSkill>());

    // Act
    await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert
    _pdfExtractor.Received(1).ExtractText(doc.FilePath);
  }

  [Fact]
  public async Task Handle_HappyPath_CallsExtractSkillsWithExtractedText()
  {
    // Arrange
    const string extractedText = "I am an expert Python developer";
    var doc = CreateDocument();
    SetupDocument(doc);
    _pdfExtractor.ExtractText(Arg.Any<string>()).Returns(extractedText);
    _skillExtractor.ExtractSkills(Arg.Any<string>()).Returns(Array.Empty<ExtractedSkill>());

    // Act
    await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert
    _skillExtractor.Received(1).ExtractSkills(extractedText);
  }

  [Fact]
  public async Task Handle_ExtractorThrows_StatusSetToFailedAndExceptionNotRethrown()
  {
    // Arrange
    var doc = CreateDocument();
    SetupDocument(doc);
    _pdfExtractor.ExtractText(Arg.Any<string>()).Throws(new InvalidOperationException("Extraction exploded"));

    // Act
    var act = async () => await _sut.Handle(new ProcessDocumentCommand(doc.Id), CancellationToken.None);

    // Assert
    await act.Should().NotThrowAsync("exceptions must be swallowed and status set to Failed");
    doc.Status.Should().Be(DocumentStatus.Failed);
    doc.ErrorMessage.Should().Contain("Extraction exploded");
  }

  [Fact]
  public async Task Handle_DocumentNotFound_ReturnsWithoutCrashing()
  {
    // Arrange — no document in repo
    _documentRepo.GetByIdAsync(DocumentId, Arg.Any<CancellationToken>())
                 .Returns((Document?)null);

    // Act
    var act = async () => await _sut.Handle(new ProcessDocumentCommand(DocumentId), CancellationToken.None);

    // Assert
    await act.Should().NotThrowAsync();
    _pdfExtractor.DidNotReceive().ExtractText(Arg.Any<string>());
  }
}
