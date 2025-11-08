using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Models;
using Service.FullTextSearch.Infrastructure.Services;

namespace Service.FullTextSearch.Tests.Services;

public class FrequencyBasedScorerTests
{
    private readonly ISearchScorer _sut;
    private readonly IInvertedIndexDocumentRetriever _invertedIndexDocumentRetriever;

    public FrequencyBasedScorerTests()
    {  
        _invertedIndexDocumentRetriever = Substitute.For<IInvertedIndexDocumentRetriever>();
        _sut = new FrequencyBasedScorer(_invertedIndexDocumentRetriever);
    }
    
    [Fact]
    public void Scorer_ShouldReturnDocumentSearchScore_WhenValidInputWithOneIndice()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var index1 = Substitute.For<InvertedIndex>("term");
        _invertedIndexDocumentRetriever.GetDocumentIds(index1).Returns(new[] { documentId1, documentId2 });
        _invertedIndexDocumentRetriever.GetFrequency(index1, documentId1).Returns(3);
        _invertedIndexDocumentRetriever.GetFrequency(index1, documentId2).Returns(1);


        var indices = new[] { index1 };

        var expected = new[]
        {
            new ScoredDocument(documentId1, 3),
            new ScoredDocument(documentId2, 1)
        };
        
        // Act
        var result = _sut.Score(indices);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Scorer_ShouldReturnDocumentSearchScore_WhenValidInputWithMultipleIndices()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var index1 = Substitute.For<InvertedIndex>("firstTerm");
        _invertedIndexDocumentRetriever.GetDocumentIds(index1).Returns(new[] { documentId1, documentId2 });
        _invertedIndexDocumentRetriever.GetFrequency(index1, documentId1).Returns(3);
        _invertedIndexDocumentRetriever.GetFrequency(index1, documentId2).Returns(1);

        var index2 = Substitute.For<InvertedIndex>("secondTerm");
        _invertedIndexDocumentRetriever.GetDocumentIds(index2).Returns(new[] { documentId2, documentId3 });
        _invertedIndexDocumentRetriever.GetFrequency(index2, documentId2).Returns(2);
        _invertedIndexDocumentRetriever.GetFrequency(index2, documentId3).Returns(5);
        
        var indices = new[] { index1, index2 };

        var expected = new[]
        {
            new ScoredDocument(documentId3, 5),
            new ScoredDocument(documentId2, 3),
            new ScoredDocument(documentId1, 3)
        };
        
        // Act
        var result = _sut.Score(indices);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Scorer_ShouldReturnEmpty_WhenNoIndices()
    {
        // Arrange
        var indices = Enumerable.Empty<InvertedIndex>();

        // Act
        var result = _sut.Score(indices);

        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void Scorer_ShouldHandle_WhenIndexHasNoDocuments()
    {
        // Arrange
        var index = Substitute.For<InvertedIndex>("term");
        _invertedIndexDocumentRetriever.GetDocumentIds(index).Returns(Enumerable.Empty<Guid>());
    
        var indices = new[] { index };

        // Act
        var result = _sut.Score(indices);

        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void Scorer_ShouldSumFrequencies_WhenSameDocumentInMultipleIndices()
    {
        // Arrange
        var documentId = Guid.NewGuid();
    
        var index1 = Substitute.For<InvertedIndex>("term1");
        var index2 = Substitute.For<InvertedIndex>("term2");
    
        _invertedIndexDocumentRetriever.GetDocumentIds(index1).Returns(new[] { documentId });
        _invertedIndexDocumentRetriever.GetDocumentIds(index2).Returns(new[] { documentId });
    
        _invertedIndexDocumentRetriever.GetFrequency(index1, documentId).Returns(3);
        _invertedIndexDocumentRetriever.GetFrequency(index2, documentId).Returns(2);

        var indices = new[] { index1, index2 };

        var expected = new[] { new ScoredDocument(documentId, 5) };

        // Act
        var result = _sut.Score(indices);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Scorer_ShouldHandle_WhenDocumentHasZeroFrequency()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var index = Substitute.For<InvertedIndex>("term");
    
        _invertedIndexDocumentRetriever.GetDocumentIds(index).Returns(new[] { documentId });
        _invertedIndexDocumentRetriever.GetFrequency(index, documentId).Returns(0);

        var indices = new[] { index };

        var expected = new[] { new ScoredDocument(documentId, 0) };

        // Act
        var result = _sut.Score(indices);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Scorer_ShouldMaintainStableOrder_WhenDocumentsHaveSameScore()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
    
        var index = Substitute.For<InvertedIndex>("term");
        _invertedIndexDocumentRetriever.GetDocumentIds(index).Returns(new[] { documentId1, documentId2, documentId3 });
    
        _invertedIndexDocumentRetriever.GetFrequency(index, documentId1).Returns(5);
        _invertedIndexDocumentRetriever.GetFrequency(index, documentId2).Returns(5);
        _invertedIndexDocumentRetriever.GetFrequency(index, documentId3).Returns(5);

        var indices = new[] { index };

        // Act
        var result = _sut.Score(indices).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.All(x => x.Score == 5).Should().BeTrue();
    }
}