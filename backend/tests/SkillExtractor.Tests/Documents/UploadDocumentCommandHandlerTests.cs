using FluentAssertions;
using MediatR;
using NSubstitute;
using SkillExtractor.Application.Documents.Commands.ProcessDocument;
using SkillExtractor.Application.Documents.Commands.UploadDocument;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Tests.Documents;

public class UploadDocumentCommandHandlerTests
{
  private readonly IDocumentRepository _documentRepository;
  private readonly IFileStorageService _fileStorageService;
  private readonly IMediator _mediator;
  private readonly UploadDocumentCommandHandler _sut;

  public UploadDocumentCommandHandlerTests()
  {
    _documentRepository = Substitute.For<IDocumentRepository>();
    _fileStorageService = Substitute.For<IFileStorageService>();
    _mediator = Substitute.For<IMediator>();

    _sut = new UploadDocumentCommandHandler(
        _documentRepository,
        _fileStorageService,
        _mediator);
  }

  [Fact]
  public async Task Handle_ValidCommand_SavesDocumentWithPendingStatus()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var storedPath = $"/uploads/{userId}/abc-CV.pdf";

    _documentRepository
        .GetActiveByUserAndTypeAsync(userId, DocumentType.CV, Arg.Any<CancellationToken>())
        .Returns([]);

    _fileStorageService
        .SaveAsync(userId, Arg.Any<Stream>(), DocumentType.CV, Arg.Any<CancellationToken>())
        .Returns(storedPath);

    var command = BuildCommand(userId, DocumentType.CV);

    // Act
    var result = await _sut.Handle(command, CancellationToken.None);

    // Assert
    result.Status.Should().Be("Pending");
    result.DocumentId.Should().NotBeEmpty();
  }

  [Fact]
  public async Task Handle_ValidCommand_ReturnsDocumentIdAndPendingStatus()
  {
    // Arrange
    var userId = Guid.NewGuid();

    _documentRepository
        .GetActiveByUserAndTypeAsync(Arg.Any<Guid>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns([]);

    _fileStorageService
        .SaveAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns("/uploads/test/file.pdf");

    var command = BuildCommand(userId, DocumentType.CV);

    // Act
    var result = await _sut.Handle(command, CancellationToken.None);

    // Assert
    result.DocumentId.Should().NotBeEmpty();
    result.Status.Should().Be("Pending");
  }

  [Fact]
  public async Task Handle_ValidCommand_CallsFileStorageServiceSaveWithUserId()
  {
    // Arrange
    var userId = Guid.NewGuid();

    _documentRepository
        .GetActiveByUserAndTypeAsync(Arg.Any<Guid>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns([]);

    _fileStorageService
        .SaveAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns("/uploads/file.pdf");

    var command = BuildCommand(userId, DocumentType.CV);

    // Act
    await _sut.Handle(command, CancellationToken.None);

    // Assert
    await _fileStorageService.Received(1)
        .SaveAsync(userId, Arg.Any<Stream>(), DocumentType.CV, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ValidCommand_DispatchesProcessDocumentCommand()
  {
    // Arrange
    var userId = Guid.NewGuid();

    _documentRepository
        .GetActiveByUserAndTypeAsync(Arg.Any<Guid>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns([]);

    _fileStorageService
        .SaveAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns("/uploads/file.pdf");

    var command = BuildCommand(userId, DocumentType.CV);

    // Act
    await _sut.Handle(command, CancellationToken.None);

    // Assert
    await _mediator.Received(1).Send(
        Arg.Any<ProcessDocumentCommand>(),
        Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_WhenExistingActiveDocumentExists_DeactivatesIt()
  {
    // Arrange
    var userId = Guid.NewGuid();

    // Simulate an existing active document for same user+type
    var existingDoc = Document.Create(userId, "old.pdf", "/uploads/old.pdf", DocumentType.CV);

    _documentRepository
        .GetActiveByUserAndTypeAsync(userId, DocumentType.CV, Arg.Any<CancellationToken>())
        .Returns(new List<Document> { existingDoc });

    _fileStorageService
        .SaveAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns("/uploads/new.pdf");

    var command = BuildCommand(userId, DocumentType.CV);

    // Act
    await _sut.Handle(command, CancellationToken.None);

    // Assert — the old document was deactivated
    existingDoc.IsActive.Should().BeFalse();
    await _documentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  // ── Helpers ──────────────────────────────────────────────────────────────

  private static UploadDocumentCommand BuildCommand(Guid userId, DocumentType documentType)
  {
    var content = new MemoryStream([0x25, 0x50, 0x44, 0x46, 0x00]); // %PDF
    return new UploadDocumentCommand(
        UserId: userId,
        FileName: "cv.pdf",
        ContentType: "application/pdf",
        FileSize: content.Length,
        FileContent: content,
        DocumentType: documentType);
  }
}
