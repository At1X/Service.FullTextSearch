using FluentAssertions;
using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services;

public class InvertedIndexDocumentRetrieverTests
{
    private readonly InvertedIndexDocumentRetriever _sut;
    
    public InvertedIndexDocumentRetrieverTests()
    {
        _sut = new InvertedIndexDocumentRetriever();
    }

    [Fact]
    public void GetFrequency_ShouldReturnFrequencyWhenPositive()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;
        fistInvertedIndex.DocumentFrequency[documentId2] = 4;

        int expectedFrequency = 3;
        
        // Act
        var result = _sut.GetFrequency(fistInvertedIndex, documentId);
        
        // Assert
        result.Should().Be(expectedFrequency);
    }
    
    [Fact]
    public void GetFrequency_ShouldReturnZeroWhenThereIsNoValue()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;

        int expectedFrequency = 0;
        
        // Act
        var result = _sut.GetFrequency(fistInvertedIndex, documentId2);
        
        // Assert
        result.Should().Be(expectedFrequency);
    }

    [Fact]
    public void GetFrequency_ShouldReturnZero_WhenDocumentFrequencyIsEmpty()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var invertedIndex = new InvertedIndex("term");
        invertedIndex.DocumentFrequency.Clear();

        // Act
        var result = _sut.GetFrequency(invertedIndex, documentId);

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void GetDocumentIds_ShouldReturnAllDocumentIds_WhenValidItemsAreInIndexDocuments()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;
        fistInvertedIndex.DocumentFrequency[documentId2] = 4;

        IReadOnlyList<Guid> expectedIds = (new[] { documentId, documentId2 });
        
        
        // Act
        var result = _sut.GetDocumentIds(fistInvertedIndex);
        
        // Assert
        result.Should().BeEquivalentTo(expectedIds);
    }
    
    [Fact]
    public void GetDocumentIds_ShouldReturnEmptyArray_WhenIndexDocumentsAreEmpty()
    {
        // Arrange
        var fistInvertedIndex = new InvertedIndex("term");
        
        // Act
        var result = _sut.GetDocumentIds(fistInvertedIndex);
        
        // Assert
        result.Should().BeEquivalentTo(Enumerable.Empty<Guid>());
    }
    
    
}