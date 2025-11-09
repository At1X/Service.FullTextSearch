using FluentAssertions;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Abstraction;
using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services;

public class InvertedIndexDocumentUpdaterTests
{
    private readonly IInvertedIndexDocumentUpdater _sut;

    public InvertedIndexDocumentUpdaterTests()
    {
        _sut = new InvertedIndexDocumentUpdater();
    }

    [Fact]
    public void AddOrUpdateDocument_ShouldUpdateADocumentsFrequency_WhenValidInputAndExistingDocument()  
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;
        fistInvertedIndex.DocumentFrequency[documentId2] = 4;
        int expectedFrequency = 6;
        
        // Act
        _sut.AddOrUpdateDocument(fistInvertedIndex, documentId, expectedFrequency);
        
        // Assert
        fistInvertedIndex.DocumentFrequency[documentId].Should().Be(expectedFrequency);
    }
    
    [Fact]
    public void AddOrUpdateDocument_ShouldNotChangeDocumentFrequency_WhenAnotherDocumentChange()  
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;
        fistInvertedIndex.DocumentFrequency[documentId2] = 4;
        int expectedFrequency = 4;
        
        // Act
        _sut.AddOrUpdateDocument(fistInvertedIndex, documentId, 6);
        
        // Assert
        fistInvertedIndex.DocumentFrequency[documentId2].Should().Be(expectedFrequency);
    }
    
    [Fact]
    public void AddOrUpdateDocument_ShouldAddNewDocumentWithItsFrequency_WhenValidInputAndNonExistingDocument()  
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;
        int expectedFrequency = 6;
        
        // Act
        _sut.AddOrUpdateDocument(fistInvertedIndex, documentId2, expectedFrequency);
        
        // Assert
        fistInvertedIndex.DocumentFrequency[documentId2].Should().Be(expectedFrequency);
    }
    
    [Fact]
    public void AddOrUpdateDocument_ShouldIncreateNumberOfElementsInDocumentFrequency_WhenValidInputAndNonExistingDocument()  
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var fistInvertedIndex = new InvertedIndex("term");
        fistInvertedIndex.DocumentFrequency[documentId] = 3;
        
        // Act
        _sut.AddOrUpdateDocument(fistInvertedIndex, documentId2, 6);
        
        // Assert
        fistInvertedIndex.DocumentFrequency.Count.Should().Be(2);
    }
    
    [Fact]
    public void AddOrUpdateDocument_ShouldThrowArgumentException_WhenFrequencyIsZero()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var invertedIndex = new InvertedIndex("term");
        int invalidFrequency = 0;

        // Act
        var act = () => _sut.AddOrUpdateDocument(invertedIndex, documentId, invalidFrequency);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Frequency must be positive (Parameter 'frequency')");
    }
    
    [Fact]
    public void AddOrUpdateDocument_ShouldThrowArgumentException_WhenFrequencyIsNegative()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var invertedIndex = new InvertedIndex("term");
        int invalidFrequency = -1;

        // Act
        var act = () => _sut.AddOrUpdateDocument(invertedIndex, documentId, invalidFrequency);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Frequency must be positive (Parameter 'frequency')");
    }
}