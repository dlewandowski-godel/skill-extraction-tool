using FluentAssertions;
using MediatR;
using NSubstitute;
using SkillExtractor.Application.Documents.Commands.UploadDocument;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Tests.Documents;

/// <summary>
/// Tests for US-3.2 — Upload IFU: verifies that the same upload endpoint
/// handles IFU documents and enforces the one-active-per-type constraint.
/// </summary>
public class UploadIfuDocumentTests
{
  private readonly IDocumentRepository _documentRepository;
  private readonly IFileStorageService _fileStorageService;
  private readonly IMediator _mediator;
  private readonly UploadDocumentCommandHandler _sut;

  public UploadIfuDocumentTests()
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
  public async Task Handle_IfuDocument_CreatesDocumentWithIfuType()
  {
    // Arrange
    var userId = Guid.NewGuid();

    _documentRepository
        .GetActiveByUserAndTypeAsync(userId, DocumentType.IFU, Arg.Any<CancellationToken>())
        .Returns([]);

    _fileStorageService
        .SaveAsync(userId, Arg.Any<Stream>(), DocumentType.IFU, Arg.Any<CancellationToken>())
        .Returns($"/uploads/{userId}/abc-IFU.pdf");

    var command = BuildCommand(userId, DocumentType.IFU);

    // Act
    var result = await _sut.Handle(command, CancellationToken.None);

    // Assert
    result.Status.Should().Be("Pending");
    result.DocumentId.Should().NotBeEmpty();

    // Verify file storage was called with IFU type
    await _fileStorageService.Received(1)
        .SaveAsync(userId, Arg.Any<Stream>(), DocumentType.IFU, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_SecondIfuUpload_DeactivatesPreviousAndCreatesNew()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var existingIfu = Document.Create(userId, "old-ifu.pdf", "/uploads/old-ifu.pdf", DocumentType.IFU);

    _documentRepository
        .GetActiveByUserAndTypeAsync(userId, DocumentType.IFU, Arg.Any<CancellationToken>())
        .Returns([existingIfu]);

    _fileStorageService
        .SaveAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns("/uploads/new-ifu.pdf");

    var command = BuildCommand(userId, DocumentType.IFU);

    // Act
    var result = await _sut.Handle(command, CancellationToken.None);

    // Assert — old IFU was deactivated
    existingIfu.IsActive.Should().BeFalse();
    result.Status.Should().Be("Pending");
  }

  [Fact]
  public async Task Handle_CvAndIfuUpload_AreTrackedIndependently()
  {
    // Arrange — user has an active CV but no active IFU
    var userId = Guid.NewGuid();
    var existingCv = Document.Create(userId, "cv.pdf", "/uploads/cv.pdf", DocumentType.CV);

    // IFU query returns nothing; CV query returns existing CV
    _documentRepository
        .GetActiveByUserAndTypeAsync(userId, DocumentType.IFU, Arg.Any<CancellationToken>())
        .Returns([]);

    _documentRepository
        .GetActiveByUserAndTypeAsync(userId, DocumentType.CV, Arg.Any<CancellationToken>())
        .Returns([existingCv]);

    _fileStorageService
        .SaveAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<DocumentType>(), Arg.Any<CancellationToken>())
        .Returns("/uploads/ifu.pdf");

    var command = BuildCommand(userId, DocumentType.IFU);

    // Act
    var result = await _sut.Handle(command, CancellationToken.None);

    // Assert — existing CV was NOT touched when uploading IFU
    existingCv.IsActive.Should().BeTrue();
    result.Status.Should().Be("Pending");
  }

  // ── Helpers ──────────────────────────────────────────────────────────────

  private static UploadDocumentCommand BuildCommand(Guid userId, DocumentType documentType)
  {
    var content = new MemoryStream([0x25, 0x50, 0x44, 0x46, 0x00]); // %PDF
    return new UploadDocumentCommand(
        UserId: userId,
        FileName: "ifu.pdf",
        ContentType: "application/pdf",
        FileSize: content.Length,
        FileContent: content,
        DocumentType: documentType);
  }
}
