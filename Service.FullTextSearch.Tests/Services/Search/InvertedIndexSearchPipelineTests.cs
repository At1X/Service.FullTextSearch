using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Application.Services.Search.Business;
using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services.Search;

public class InvertedIndexSearchPipelineTests
{
    private readonly ISearchPipeline _sut;
    private readonly IInvertedIndexRepository  _repository;
    private readonly ITextProcessor  _textProcessor;
    private readonly ISearchScoreCalculator  _searchScoreCalculator;

    public InvertedIndexSearchPipelineTests()
    {
        _repository = Substitute.For<IInvertedIndexRepository>();
        _textProcessor = Substitute.For<ITextProcessor>();
        _searchScoreCalculator = Substitute.For<ISearchScoreCalculator>();
        _sut = new InvertedIndexSearchPipeline(_repository, _textProcessor, _searchScoreCalculator);
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
            new ScoredDocument()
            {
                DocumentId = Guid.NewGuid(),
                Score = 8
            },
            new ScoredDocument()
            {
                DocumentId = Guid.NewGuid(),
                Score = 8
            },
        };

        _textProcessor.PreProcessText(searchText).Returns(tokens);
        _repository.SearchTerms(tokens).Returns(indices);
        _searchScoreCalculator.CalculateScore(indices).Returns(expectedResults);
        
        // Act
        _sut.Search(searchText);

        // Assert
        _textProcessor.Received(1).PreProcessText(searchText);
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
            new ScoredDocument()
            {
                DocumentId = Guid.NewGuid(),
                Score = 8
            },
            new ScoredDocument()
            {
                DocumentId = Guid.NewGuid(),
                Score = 8
            },
        };

        _textProcessor.PreProcessText(searchText).Returns(tokens);
        _repository.SearchTerms(tokens).Returns(indices);
        _searchScoreCalculator.CalculateScore(indices).Returns(expectedResults);
        
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
            new ScoredDocument()
            {
                DocumentId = Guid.NewGuid(),
                Score = 8
            },
            new ScoredDocument()
            {
                DocumentId = Guid.NewGuid(),
                Score = 8
            },
        };

        _textProcessor.PreProcessText(searchText).Returns(tokens);
        _repository.SearchTerms(tokens).Returns(indices);
        _searchScoreCalculator.CalculateScore(indices).Returns(expectedResults);
        
        // Act
        var result = _sut.Search(searchText);

        // Assert
        _searchScoreCalculator.Received(1).CalculateScore(indices);
        result.Should().BeEquivalentTo(expectedResults);
        
    }
}

