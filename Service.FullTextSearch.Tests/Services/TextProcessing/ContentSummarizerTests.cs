using FluentAssertions;
using Service.FullTextSearch.Application.Services.TextProcessing.Business;

namespace Service.FullTextSearch.Tests.Services.TextProcessing;

public class ContentSummarizerTests
{
    private readonly ContentSummarizer _sut;

    public ContentSummarizerTests()
    {
        _sut = new ContentSummarizer();
    }

    [Fact]
    public void Summarize_WhenContentIsShorterThanMaxLength_ReturnsOriginalContent()
    {
        // Arrange
        var content = "This is a short text";
        var maxLength = 50;

        // Act
        var result = _sut.Summarize(content, maxLength);

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public void Summarize_WhenContentIsEqualToMaxLength_ReturnsOriginalContent()
    {
        // Arrange
        var content = "This is exactly twenty five";
        var maxLength = content.Length;

        // Act
        var result = _sut.Summarize(content, maxLength);

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public void Summarize_WhenContentIsLongerThanMaxLength_ReturnsTruncatedContentWithEllipsis()
    {
        // Arrange
        var content = "This is a very long content that exceeds the maximum length allowed for summary";
        var maxLength = 20;

        // Act
        var result = _sut.Summarize(content, maxLength);

        // Assert
        result.Should().Be("This is a very long ...");
        result.Length.Should().Be(maxLength + 3);
    }

    [Fact]
    public void Summarize_WhenContentIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var content = string.Empty;
        var maxLength = 50;

        // Act
        var result = _sut.Summarize(content, maxLength);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Summarize_WhenMaxLengthIsZero_ReturnsEllipsisOnly()
    {
        // Arrange
        var content = "This is some content";
        var maxLength = 0;

        // Act
        var result = _sut.Summarize(content, maxLength);

        // Assert
        result.Should().Be("...");
    }
}