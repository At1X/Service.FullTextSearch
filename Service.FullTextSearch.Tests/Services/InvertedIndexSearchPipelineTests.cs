using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.Models;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Interfaces;
using Service.FullTextSearch.Infrastructure.Services;

namespace Service.FullTextSearch.Tests.Services;

public class InvertedIndexSearchPipelineTests
{
    private readonly ISearchPipeline _sut;
    private readonly IInvertedIndexRepository  _repository;
    private readonly ITextProcessor  _textProcessor;
    private readonly ISearchScorer  _searchScorer;

    public InvertedIndexSearchPipelineTests()
    {
        _repository = Substitute.For<IInvertedIndexRepository>();
        _textProcessor = Substitute.For<ITextProcessor>();
        _searchScorer = Substitute.For<ISearchScorer>();
        _sut = new InvertedIndexSearchPipeline(_repository, _textProcessor, _searchScorer);
    }
    
    [Fact]
    public void Search_ShouldProcessTextCallOnce_WhenValidSearchTextProvided()
    {
        // Arrange
        var searchText = "test query";
        var tokens = new[] { "test", "query" };
        var indices = new List<InvertedIndex>();
        var expectedResults = new List<ScoredDocument>
        {
            new(Guid.NewGuid(), 8),
            new(Guid.NewGuid(), 8)
        };

        _textProcessor.Process(searchText).Returns(tokens);
        _repository.SearchTerms(tokens).Returns(indices);
        _searchScorer.Score(indices).Returns(expectedResults);
        
        // Act
        _sut.Search(searchText);

        // Assert
        _textProcessor.Received(1).Process(searchText);
    }
    
    [Fact]
    public void Search_ShouldSearchTermsCallOnce_WhenValidSearchTextProvided()
    {
        // Arrange
        var searchText = "test query";
        var tokens = new[] { "test", "query" };
        var indices = new List<InvertedIndex>();
        var expectedResults = new List<ScoredDocument>
        {
            new(Guid.NewGuid(), 8),
            new(Guid.NewGuid(), 8)
        };

        _textProcessor.Process(searchText).Returns(tokens);
        _repository.SearchTerms(tokens).Returns(indices);
        _searchScorer.Score(indices).Returns(expectedResults);
        
        // Act
        _sut.Search(searchText);

        // Assert
        _repository.Received(1).SearchTerms(tokens);
    }
    
    [Fact]
    public void Search_ShouldScorerCallOnce_WhenValidSearchTextProvided()
    {
        // Arrange
        var searchText = "test query";
        var tokens = new[] { "test", "query" };
        var indices = new List<InvertedIndex>();
        var expectedResults = new List<ScoredDocument>
        {
            new(Guid.NewGuid(), 8),
            new(Guid.NewGuid(), 8)
        };

        _textProcessor.Process(searchText).Returns(tokens);
        _repository.SearchTerms(tokens).Returns(indices);
        _searchScorer.Score(indices).Returns(expectedResults);
        
        // Act
        var result = _sut.Search(searchText);

        // Assert
        _searchScorer.Received(1).Score(indices);
        result.Should().BeEquivalentTo(expectedResults);
        
    }
}

