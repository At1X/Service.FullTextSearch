using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Builders;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Application.SearchResultBuilder.Abstraction;
using Service.FullTextSearch.Application.SearchResultBuilder.Business;
using Service.FullTextSearch.Application.SearchResultMapper.Abstraction;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Tests.Services;

public class SearchResultBuilderTests
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ISearchResultMapper _resultMapper;
    private readonly ISearchResultBuilder _sut;


    public SearchResultBuilderTests()
    {
        _documentRepository = Substitute.For<IDocumentRepository>();
        _resultMapper = Substitute.For<ISearchResultMapper>();
        _sut = new SearchResultBuilder(_documentRepository, _resultMapper);
    }

    [Fact]
    public void BuildResults_ShouldReturnMappedResults_WhenAllDocumentsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var scoredDocuments = new List<ScoredDocument>
        {
            new(documentId1, 10),
            new(documentId2, 12)
        };

        var document1 = new DocumentBuilder()
            .WithTitle("test title 1")
            .WithContent("test content 1")
            .Build();;
        var document2 = new DocumentBuilder()
            .WithTitle("test title 2")
            .WithContent("test content 2")
            .Build();;

        var resultDto1 = new DocumentResultDto(documentId1, "Title 1", "Content 1", 10);
        var resultDto2 = new DocumentResultDto(documentId2, "Title 2", "Content 2", 12);

        _documentRepository.GetById(documentId1).Returns(document1);
        _documentRepository.GetById(documentId2).Returns(document2);
        _resultMapper.MapToDto(document1, 10).Returns(resultDto1);
        _resultMapper.MapToDto(document2, 12).Returns(resultDto2);

        var expectedResults = new List<DocumentResultDto> { resultDto1, resultDto2 };

        // Act
        var results = _sut.BuildResults(scoredDocuments);

        // Assert
        results.Should().BeEquivalentTo(expectedResults);
        _documentRepository.Received(1).GetById(documentId1);
        _documentRepository.Received(1).GetById(documentId2);
        _resultMapper.Received(1).MapToDto(document1, 10);
        _resultMapper.Received(1).MapToDto(document2, 12);
    }

    [Fact]
    public void BuildResults_ShouldReturnEmptyList_WhenNoScoredDocumentsProvided()
    {
        // Arrange
        var scoredDocuments = Array.Empty<ScoredDocument>();

        // Act
        var results = _sut.BuildResults(scoredDocuments);

        // Assert
        results.Should().BeEmpty();
        _documentRepository.DidNotReceiveWithAnyArgs().GetById(Arg.Any<Guid>());
        _resultMapper.DidNotReceiveWithAnyArgs().MapToDto(Arg.Any<Document>(), Arg.Any<int>());
    }
}