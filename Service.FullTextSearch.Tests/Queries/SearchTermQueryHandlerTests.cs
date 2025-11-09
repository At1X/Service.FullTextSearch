using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.MedatorActions.Queries;
using Service.FullTextSearch.Application.SearchQueryPipeline.Abstraction;
using Service.FullTextSearch.Application.SearchResultBuilder.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Queries;

public class SearchTermQueryHandlerTests
{
    private readonly ISearchPipeline _searchPipeline;
    private readonly ISearchResultBuilder _resultBuilder;
    private readonly IRequestHandler<SearchTermQuery, Result<SearchResultDto>> _sut;

    public SearchTermQueryHandlerTests()
    {
        _searchPipeline = Substitute.For<ISearchPipeline>();
        _resultBuilder = Substitute.For<ISearchResultBuilder>();
        _sut = new SearchTermQueryHandler(_searchPipeline, _resultBuilder);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithSearchResults_WhenSearchReturnsDocuments()
    {
        // Arrange
        var searchText = "test query";
        var query = new SearchTermQuery(searchText);
        
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        
        var scoredDocuments = new List<ScoredDocument>
        {
            new ScoredDocument()
            {
                DocumentId = documentId1,
                Score = 12
            },
            new  ScoredDocument()
            {
                DocumentId = documentId2,   
                Score = 13
            }
        };

        var resultDto1 = new DocumentResultDto(documentId1, "Title 1", "Content 1", 10);
        var resultDto2 = new DocumentResultDto(documentId2, "Title 2", "Content 2", 12);
        var builtResults = new List<DocumentResultDto> { resultDto1, resultDto2 };

        _searchPipeline.Search(searchText).Returns(scoredDocuments);
        _resultBuilder.BuildResults(scoredDocuments).Returns(builtResults);

        var expectedSearchResult = new SearchResultDto(builtResults, 2, searchText);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedSearchResult);
        _searchPipeline.Received(1).Search(searchText);
        _resultBuilder.Received(1).BuildResults(scoredDocuments);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithEmptyResults_WhenNoDocumentsFound()
    {
        // Arrange
        var searchText = "nonexistent term";
        var query = new SearchTermQuery(searchText);
        
        var scoredDocuments = new List<ScoredDocument>();
        var builtResults = new List<DocumentResultDto>();

        _searchPipeline.Search(searchText).Returns(scoredDocuments);
        _resultBuilder.BuildResults(scoredDocuments).Returns(builtResults);

        var expectedSearchResult = new SearchResultDto(builtResults, 0, searchText);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedSearchResult);
        _searchPipeline.Received(1).Search(searchText);
        _resultBuilder.Received(1).BuildResults(scoredDocuments);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSearchPipelineThrowsException()
    {
        // Arrange
        var searchText = "failing query";
        var query = new SearchTermQuery(searchText);
        
        var exception = new Exception("Search pipeline error");
        _searchPipeline.Search(searchText).Throws(exception);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Search failed: Search pipeline error");
        _searchPipeline.Received(1).Search(searchText);
        _resultBuilder.DidNotReceiveWithAnyArgs().BuildResults(Arg.Any<IReadOnlyCollection<ScoredDocument>>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenResultBuilderThrowsException()
    {
        // Arrange
        var searchText = "builder failure";
        var query = new SearchTermQuery(searchText);
        
        var scoredDocuments = new List<ScoredDocument> { new ScoredDocument()
        {
            DocumentId = Guid.NewGuid(),
            Score = 12
        } };

        _searchPipeline.Search(searchText).Returns(scoredDocuments);
        _resultBuilder.BuildResults(scoredDocuments).Throws(new Exception("Builder processing failed"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Search failed: Builder processing failed");
        _searchPipeline.Received(1).Search(searchText);
        _resultBuilder.Received(1).BuildResults(scoredDocuments);
    }
    
}