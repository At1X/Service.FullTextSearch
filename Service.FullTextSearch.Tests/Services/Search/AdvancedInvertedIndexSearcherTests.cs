using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Application.Services.Search.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services.Search;

public class AdvancedInvertedIndexSearcherTests
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IQueryParser _queryParser;
    private readonly ISearchScoreCalculator _scoreCalculator;
    private readonly ISearchFilterAggregator _searchFilterAggregator;
    private readonly AdvancedInvertedIndexSearcher _sut;

    public AdvancedInvertedIndexSearcherTests()
    {
        _indexRepository = Substitute.For<IInvertedIndexRepository>();
        _queryParser = Substitute.For<IQueryParser>();
        _scoreCalculator = Substitute.For<ISearchScoreCalculator>();
        _searchFilterAggregator = Substitute.For<ISearchFilterAggregator>();
        _sut = new AdvancedInvertedIndexSearcher(
            _indexRepository,
            _queryParser,
            _scoreCalculator,
            _searchFilterAggregator);
    }

    [Fact]
    public void Search_ShouldReturnFilteredDocuments_WhenValidSearchTextProvided()
    {
        // Arrange
        var searchText = "hello world";
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();

        var parsedQuery = new ParsedQuery()
        {
            RequiredTerms = new List<string> { "hello" },
            OptionalTerms = new List<string> { "world" },
            ExcludedTerms = new List<string>()
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "hello", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "world", DocumentFrequency = new Dictionary<Guid, int> { { documentId2, 1 } } };
        var indices = new List<InvertedIndex> { indexEntry1, indexEntry2 };

        var scoredDocument1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var scoredDocument2 = new ScoredDocument { DocumentId = documentId2, Score = 8 };
        var scoredDocuments = new List<ScoredDocument> { scoredDocument1, scoredDocument2 };

        var filteredDocuments = new List<ScoredDocument> { scoredDocument1 };

        _queryParser.Parse(searchText).Returns(parsedQuery);
        _indexRepository.SearchTerms(Arg.Is<List<string>>(list => 
            list.Contains("hello") && list.Contains("world"))).Returns(indices);
        _scoreCalculator.CalculateScore(indices).Returns(scoredDocuments);
        _searchFilterAggregator.Filter(scoredDocuments, parsedQuery).Returns(filteredDocuments);

        // Act
        var result = _sut.Search(searchText);

        // Assert
        result.Should().BeEquivalentTo(filteredDocuments);
        _queryParser.Received(1).Parse(searchText);
        _indexRepository.Received(1).SearchTerms(Arg.Any<List<string>>());
        _scoreCalculator.Received(1).CalculateScore(indices);
        _searchFilterAggregator.Received(1).Filter(scoredDocuments, parsedQuery);
    }

    [Fact]
    public void Search_ShouldReturnEmpty_WhenNoTermsInParsedQuery()
    {
        // Arrange
        var searchText = "";
        var parsedQuery = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            OptionalTerms = new List<string>(),
            ExcludedTerms = new List<string>()
        };

        _queryParser.Parse(searchText).Returns(parsedQuery);

        // Act
        var result = _sut.Search(searchText);

        // Assert
        result.Should().BeEmpty();
        _queryParser.Received(1).Parse(searchText);
        _indexRepository.DidNotReceive().SearchTerms(Arg.Any<List<string>>());
        _scoreCalculator.DidNotReceive().CalculateScore(Arg.Any<IReadOnlyCollection<InvertedIndex>>());
        _searchFilterAggregator.DidNotReceive().Filter(Arg.Any<IReadOnlyCollection<ScoredDocument>>(), Arg.Any<ParsedQuery>());
    }
}