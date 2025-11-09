using FluentAssertions;
using Service.FullTextSearch.Application.Common.EntityBuilder;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Infrastructure.Persistence.Repositories;

namespace Service.FullTextSearch.Tests.Repositories;

public class InMemoryDocumentRepositoryTests
{
    private readonly IDocumentRepository _sut;

    public InMemoryDocumentRepositoryTests()
    {
        _sut = new InMemoryDocumentRepository();
    }

    [Fact]
    public void GetById_ShouldReturnDocument_WhenDocumentExists()
    {
        // Arrange
        var document = new DocumentBuilder()
            .WithTitle("test title")
            .WithContent("test content")
            .Build();
        _sut.Add(document);

        // Act
        var result = _sut.GetById(document.Id);

        // Assert
        result.Should().BeEquivalentTo(document);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenDocumentDoesNotExist()
    {
        // Arrange
        var documentId = Guid.NewGuid();

        // Act
        var result = _sut.GetById(documentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetAll_ShouldReturnAllDocuments_WhenDocumentsExist()
    {
        // Arrange
        var document1 = new DocumentBuilder()
            .WithTitle("test title")
            .WithContent("test content")
            .Build();;
        var document2 = new DocumentBuilder()
            .WithTitle("test title 2")
            .WithContent("test content 2")
            .Build();;
        var expected = new List<Document> { document1, document2 };

        _sut.Add(document1);
        _sut.Add(document2);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetAll_ShouldReturnEmptyList_WhenNoDocumentsExist()
    {
        // Arrange

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Add_ShouldStoreDocument_WhenNewDocumentAdded()
    {
        // Arrange
        var document = new DocumentBuilder()
            .WithTitle("test title 1")
            .WithContent("test content")
            .Build();;

        // Act
        var result = _sut.Add(document);

        // Assert
        result.Should().BeEquivalentTo(document);
        _sut.GetById(document.Id).Should().BeEquivalentTo(document);
    }

    [Fact]
    public void Update_ShouldAddDocument_WhenDocumentDoesNotExist()
    {
        // Arrange
        var document = new DocumentBuilder()
            .WithTitle("test title 1")
            .WithContent("test content")
            .Build();;

        // Act
        _sut.Update(document);

        // Assert
        _sut.GetById(document.Id).Should().BeEquivalentTo(document);
    }

    [Fact]
    public void Delete_ShouldRemoveDocument_WhenDocumentExists()
    {
        // Arrange
        var document = new DocumentBuilder()
            .WithTitle("test title")
            .WithContent("test content")
            .Build();;
        _sut.Add(document);

        // Act
        _sut.Delete(document.Id);

        // Assert
        _sut.GetById(document.Id).Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldDoNothing_WhenDocumentDoesNotExist()
    {
        // Arrange
        var documentId = Guid.NewGuid();

        // Act
        _sut.Delete(documentId);

        // Assert
        _sut.GetById(documentId).Should().BeNull();
    }
}