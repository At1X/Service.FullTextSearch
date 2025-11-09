using FluentAssertions;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Infrastructure.Services;

namespace Service.FullTextSearch.Tests.Services;


public class StopWordRemoverTests
{
    private readonly IStopWordRemover _sut;

    public StopWordRemoverTests()
    {
        _sut = new StopWordRemover();
    }

    [Fact]
    public void RemoveStopWords_ShouldReturnOnlyNonStopWords_WhenInputContainsStopWords()
    {
        // Arrange
        var tokens = new List<string> { "the", "quick", "brown", "fox", "and", "the", "dog" };
        var expected = new List<string> { "quick", "brown", "fox", "dog" };

        // Act
        var result = _sut.RemoveStopWords(tokens);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void RemoveStopWords_ShouldReturnAllTokens_WhenNoStopWordsPresent()
    {
        // Arrange
        var tokens = new List<string> { "quick", "brown", "fox", "jumps" };
        var expected = new List<string> { "quick", "brown", "fox", "jumps" };

        // Act
        var result = _sut.RemoveStopWords(tokens);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void RemoveStopWords_ShouldReturnEmptyEnumerable_WhenAllTokensAreStopWords()
    {
        // Arrange
        var tokens = new List<string> { "the", "and", "in", "on", "at" };

        // Act
        var result = _sut.RemoveStopWords(tokens);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RemoveStopWords_ShouldHandleMixedCaseStopWords_WhenInputHasUpperCase()
    {
        // Arrange
        var tokens = new List<string> { "The", "QUICK", "Brown", "AND", "Fox" };
        var expected = new List<string> { "QUICK", "Brown", "Fox" };

        // Act
        var result = _sut.RemoveStopWords(tokens);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void RemoveStopWords_ShouldHandleEmptyInput_WhenTokensIsEmpty()
    {
        // Arrange
        var tokens = Array.Empty<string>();

        // Act
        var result = _sut.RemoveStopWords(tokens);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RemoveStopWords_ShouldRemoveCommonStopWords_WhenVariousStopWordsPresent()
    {
        // Arrange
        var tokens = new List<string> { "a", "an", "the", "and", "but", "is", "are", "was" };
        var expected = Enumerable.Empty<string>();

        // Act
        var result = _sut.RemoveStopWords(tokens);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}