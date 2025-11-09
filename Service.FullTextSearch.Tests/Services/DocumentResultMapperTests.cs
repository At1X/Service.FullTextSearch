using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Builders;
using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.ContentSummerizer.Abstraction;
using Service.FullTextSearch.Application.SearchResultMapper.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services;

public class DocumentResultMapperTests
{
    private readonly IContentSummarizer _summarizer;
    private readonly DocumentResultMapper _sut;
    
    public DocumentResultMapperTests()
    {
        _summarizer = Substitute.For<IContentSummarizer>();
        _sut = new DocumentResultMapper(_summarizer);
    }

    [Fact]
    public void MapToDto_ShouldReturnDocumentResultDto_WhenValidInput()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var document = new DocumentBuilder()
            .WithTitle(
            "Test Document")
            .WithContent(
            "This is a long content that needs to be summarized for the search results display."
            )
            .Build();
        var score = 85;
        var expectedSummary = "This is a long content that needs...";
    
        _summarizer.Summarize(document.Content, 200).Returns(expectedSummary);
        var expected = new DocumentResultDto(documentId, "Test Document", expectedSummary, score);

        // Act
        var result = _sut.MapToDto(document, score);

        // Assert
        result.Should().BeEquivalentTo(expected, options => options
            .Excluding(dto => dto.Id));
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void MapToDto_ShouldCallSummarizerWithCorrectParameters_WhenDocumentProvided()
    {
        // Arrange
        var document = new DocumentBuilder()
            .WithTitle("Another Test Document")
            .WithContent("Another long content that requires summarization for better user experience.")
            .Build();
        var score = 92;
        var expectedSummary = "Another long content that requires...";
    
        _summarizer.Summarize(document.Content, 200).Returns(expectedSummary);

        // Act
        _sut.MapToDto(document, score);

        // Assert
        _summarizer.Received(1).Summarize(document.Content, 200);
    }
}

