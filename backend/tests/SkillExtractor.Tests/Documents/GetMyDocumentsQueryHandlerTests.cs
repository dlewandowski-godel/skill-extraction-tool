using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Documents.Queries.GetMyDocuments;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Tests.Documents;

public class GetMyDocumentsQueryHandlerTests
{
  private readonly IDocumentRepository _documentRepository;
  private readonly GetMyDocumentsQueryHandler _sut;

  public GetMyDocumentsQueryHandlerTests()
  {
    _documentRepository = Substitute.For<IDocumentRepository>();
    _sut = new GetMyDocumentsQueryHandler(_documentRepository);
  }

  [Fact]
  public async Task Handle_ReturnsOnlyCurrentUsersDocuments()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var doc1 = Document.Create(userId, "cv.pdf", "/uploads/cv.pdf", DocumentType.CV);
    var doc2 = Document.Create(userId, "ifu.pdf", "/uploads/ifu.pdf", DocumentType.IFU);

    _documentRepository
        .GetByUserAsync(userId, Arg.Any<CancellationToken>())
        .Returns([doc2, doc1]); // repo returns them in descending order already

    // Act
    var result = await _sut.Handle(new GetMyDocumentsQuery(userId), CancellationToken.None);

    // Assert
    result.Should().HaveCount(2);

    // Repo was queried with the correct user id (not another user's)
    await _documentRepository.Received(1).GetByUserAsync(userId, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_DoesNotReturnOtherUsersDocuments()
  {
    // Arrange
    var requestingUser = Guid.NewGuid();
    var otherUser = Guid.NewGuid();

    // Only the requesting user's documents are returned by the repository
    var userDoc = Document.Create(requestingUser, "cv.pdf", "/uploads/cv.pdf", DocumentType.CV);

    _documentRepository
        .GetByUserAsync(requestingUser, Arg.Any<CancellationToken>())
        .Returns([userDoc]);

    // Act
    var result = await _sut.Handle(new GetMyDocumentsQuery(requestingUser), CancellationToken.None);

    // Assert — repository was ONLY called for the requesting user
    result.Should().HaveCount(1);
    result[0].DocumentId.Should().Be(userDoc.Id);
    await _documentRepository.DidNotReceive().GetByUserAsync(otherUser, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_MapsDocumentFieldsCorrectly()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var doc = Document.Create(userId, "my-cv.pdf", "/uploads/my-cv.pdf", DocumentType.CV);

    _documentRepository
        .GetByUserAsync(userId, Arg.Any<CancellationToken>())
        .Returns([doc]);

    // Act
    var result = await _sut.Handle(new GetMyDocumentsQuery(userId), CancellationToken.None);

    // Assert
    var dto = result.Single();
    dto.DocumentId.Should().Be(doc.Id);
    dto.FileName.Should().Be("my-cv.pdf");
    dto.DocumentType.Should().Be("CV");
    dto.Status.Should().Be("Pending");
    dto.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    dto.ProcessedAt.Should().BeNull();
    dto.ErrorMessage.Should().BeNull();
  }

  [Fact]
  public async Task Handle_ReturnedList_HonorsRepositoryOrdering()
  {
    // Arrange — repository returns documents in descending UploadedAt order
    var userId = Guid.NewGuid();
    var newerDoc = Document.Create(userId, "newer.pdf", "/path/newer.pdf", DocumentType.CV);
    var olderDoc = Document.Create(userId, "older.pdf", "/path/older.pdf", DocumentType.IFU);

    // Simulate repository returning newer first (descending order)
    _documentRepository
        .GetByUserAsync(userId, Arg.Any<CancellationToken>())
        .Returns([newerDoc, olderDoc]);

    // Act
    var result = await _sut.Handle(new GetMyDocumentsQuery(userId), CancellationToken.None);

    // Assert — handler preserves order from repository
    result.Should().HaveCount(2);
    result[0].DocumentId.Should().Be(newerDoc.Id);
    result[1].DocumentId.Should().Be(olderDoc.Id);
  }

  [Fact]
  public async Task Handle_WhenNoDocuments_ReturnsEmptyList()
  {
    // Arrange
    var userId = Guid.NewGuid();

    _documentRepository
        .GetByUserAsync(userId, Arg.Any<CancellationToken>())
        .Returns([]);

    // Act
    var result = await _sut.Handle(new GetMyDocumentsQuery(userId), CancellationToken.None);

    // Assert
    result.Should().BeEmpty();
  }
}
