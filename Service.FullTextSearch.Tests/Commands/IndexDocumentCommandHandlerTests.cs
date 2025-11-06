using FluentAssertions;
using Moq;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.Commands;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Interfaces;
using Xunit;

namespace Service.FullTextSearch.Tests.Commands;

public class IndexDocumentCommandHandlerTests
{
    private readonly Mock<IDocumentRepository> _documentRepositoryMock;
    private readonly Mock<IInvertedIndexRepository> _indexRepositoryMock;
    private readonly Mock<ITokenizer> _tokenizerMock;
    private readonly Mock<IStopWordRemover> _stopWordRemoverMock;
    private readonly IndexDocumentCommandHandler _sut;

    public IndexDocumentCommandHandlerTests()
    {
        _documentRepositoryMock = new Mock<IDocumentRepository>();
        _indexRepositoryMock = new Mock<IInvertedIndexRepository>();
        _tokenizerMock = new Mock<ITokenizer>();
        _stopWordRemoverMock = new Mock<IStopWordRemover>();

        _sut = new IndexDocumentCommandHandler(
            _documentRepositoryMock.Object,
            _indexRepositoryMock.Object,
            _tokenizerMock.Object,
            _stopWordRemoverMock.Object);
    }

    [Fact]
    public async Task Handle_ValidDocument_ReturnsSuccessWithDocumentId()
    {
        // Arrange
        var command = new IndexDocumentCommand(
            Title: "Test Document",
            Content: "This is a test document content"
        );

        var tokens = new List<string> { "this", "is", "a", "test", "document", "content" };
        var cleanTokens = new List<string> { "test", "document", "content" };

        _tokenizerMock.Setup(x => x.Tokenize(command.Content)).Returns(tokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(tokens)).Returns(cleanTokens);
        _indexRepositoryMock.Setup(x => x.GetByTerm(It.IsAny<string>())).Returns((InvertedIndex?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        _documentRepositoryMock.Verify(x => x.Add(It.IsAny<Document>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidDocument_TokenizesContent()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "This is test content"
        );

        var tokens = new List<string> { "this", "is", "test", "content" };
        _tokenizerMock.Setup(x => x.Tokenize(command.Content)).Returns(tokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(tokens)).Returns(new List<string> { "test", "content" });

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _tokenizerMock.Verify(x => x.Tokenize(command.Content), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidDocument_RemovesStopWords()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "This is test content"
        );

        var tokens = new List<string> { "this", "is", "test", "content" };
        var cleanTokens = new List<string> { "test", "content" };

        _tokenizerMock.Setup(x => x.Tokenize(command.Content)).Returns(tokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(tokens)).Returns(cleanTokens);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _stopWordRemoverMock.Verify(x => x.RemoveStopWords(tokens), Times.Once);
    }

    [Fact]
    public async Task Handle_NewTerm_CreatesNewInvertedIndex()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "test content"
        );

        var cleanTokens = new List<string> { "test", "content" };

        _tokenizerMock.Setup(x => x.Tokenize(It.IsAny<string>())).Returns(cleanTokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(It.IsAny<List<string>>())).Returns(cleanTokens);
        _indexRepositoryMock.Setup(x => x.GetByTerm(It.IsAny<string>())).Returns((InvertedIndex?)null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _indexRepositoryMock.Verify(x => x.Add(It.IsAny<InvertedIndex>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ExistingTerm_UpdatesInvertedIndex()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "test content"
        );

        var cleanTokens = new List<string> { "test" };
        var existingIndex = new InvertedIndex("test");

        _tokenizerMock.Setup(x => x.Tokenize(It.IsAny<string>())).Returns(cleanTokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(It.IsAny<List<string>>())).Returns(cleanTokens);
        _indexRepositoryMock.Setup(x => x.GetByTerm("test")).Returns(existingIndex);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _indexRepositoryMock.Verify(x => x.Update(It.IsAny<InvertedIndex>()), Times.Once);
        _indexRepositoryMock.Verify(x => x.Add(It.IsAny<InvertedIndex>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicateTokens_CalculatesCorrectTermFrequency()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "test test test content"
        );

        var cleanTokens = new List<string> { "test", "test", "test", "content" };

        _tokenizerMock.Setup(x => x.Tokenize(It.IsAny<string>())).Returns(cleanTokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(It.IsAny<List<string>>())).Returns(cleanTokens);
        _indexRepositoryMock.Setup(x => x.GetByTerm(It.IsAny<string>())).Returns((InvertedIndex?)null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _indexRepositoryMock.Verify(x => x.Add(It.Is<InvertedIndex>(idx => 
            idx.Term == "test")), Times.Once);
        _indexRepositoryMock.Verify(x => x.Add(It.Is<InvertedIndex>(idx => 
            idx.Term == "content")), Times.Once);
    }

    [Fact]
    public async Task Handle_CaseInsensitiveTerms_NormalizesToLowerCase()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "Test TEST test"
        );

        var cleanTokens = new List<string> { "Test", "TEST", "test" };

        _tokenizerMock.Setup(x => x.Tokenize(It.IsAny<string>())).Returns(cleanTokens);
        _stopWordRemoverMock.Setup(x => x.RemoveStopWords(It.IsAny<List<string>>())).Returns(cleanTokens);
        _indexRepositoryMock.Setup(x => x.GetByTerm(It.IsAny<string>())).Returns((InvertedIndex?)null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _indexRepositoryMock.Verify(x => x.Add(It.IsAny<InvertedIndex>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var command = new IndexDocumentCommand
        (
            Title: "Test Document",
            Content: "test content"
        );

        _tokenizerMock.Setup(x => x.Tokenize(It.IsAny<string>()))
            .Throws(new Exception("Tokenization failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to index document");
        result.Error.Should().Contain("Tokenization failed");
    }
}