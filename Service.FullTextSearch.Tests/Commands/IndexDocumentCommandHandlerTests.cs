using FluentAssertions;
using MediatR;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Documents.Commands;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Commands;

public class IndexDocumentCommandHandlerTests
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ITextProcessor _textProcessor;
    private readonly IDocumentIndexer _documentIndexer;
    private readonly IRequestHandler<IndexDocumentCommand, Result<Guid>> _sut;

    public IndexDocumentCommandHandlerTests()
    {
        _documentRepository = Substitute.For<IDocumentRepository>();
        _textProcessor = Substitute.For<ITextProcessor>();
        _documentIndexer = Substitute.For<IDocumentIndexer>();
        _sut = new IndexDocumentCommandHandler(_documentRepository, _textProcessor, _documentIndexer);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithDocumentId_WhenDocumentIsIndexedSuccessfully()
    {
        // Arrange
        var command = new IndexDocumentCommand("Test Title", "Test content for indexing");
        var documentId = Guid.NewGuid();
        var termFrequencies = new Dictionary<string, int> { { "test", 1 }, { "content", 1 }, { "indexing", 1 } };

        _documentRepository.When(repo => repo.Add(Arg.Any<Document>()))
            .Do(call => { var doc = call.Arg<Document>(); doc.GetType().GetProperty("Id").SetValue(doc, documentId); });
        _textProcessor.CalculateTermFrequency(command.Content).Returns(termFrequencies);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(documentId);
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldAddDocumentToRepository_WhenCommandIsValid()
    {
        // Arrange
        var command = new IndexDocumentCommand("Test Title", "Test content");
        Document capturedDocument = null;

        _documentRepository.When(repo => repo.Add(Arg.Any<Document>()))
            .Do(call => capturedDocument = call.Arg<Document>());
        _textProcessor.CalculateTermFrequency(command.Content).Returns(new Dictionary<string, int>());

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _documentRepository.Received(1).Add(Arg.Any<Document>());
        capturedDocument.Should().NotBeNull();
        capturedDocument.Title.Should().Be(command.Title);
        capturedDocument.Content.Should().Be(command.Content);
    }

    [Fact]
    public async Task Handle_ShouldCalculateTermFrequency_WhenDocumentContentIsProvided()
    {
        // Arrange
        var command = new IndexDocumentCommand("Title", "This is test content for processing");
        var expectedTermFrequencies = new Dictionary<string, int>
        {
            { "test", 1 },
            { "content", 1 },
            { "processing", 1 }
        };

        _textProcessor.CalculateTermFrequency(command.Content).Returns(expectedTermFrequencies);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _textProcessor.Received(1).CalculateTermFrequency(command.Content);
    }
    

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccursDuringIndexing()
    {
        // Arrange
        var command = new IndexDocumentCommand("Title", "Test content");
        var exceptionMessage = "Test exception";

        _documentRepository.When(repo => repo.Add(Arg.Any<Document>()))
            .Throw(new InvalidOperationException(exceptionMessage));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to index document");
        result.Error.Should().Contain(exceptionMessage);
    }
}