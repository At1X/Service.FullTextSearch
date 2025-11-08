using FluentAssertions;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Infrastructure.Services;

namespace Service.FullTextSearch.Tests.Services;


public class TokenizerTests
{
    private readonly ITokenizer _sut;

    public TokenizerTests()
    {
        _sut = new Tokenizer();
    }

    [Fact]
    public void Tokenize_ShouldReturnWordsSplitByNonWordCharacters_WhenTextContainsPunctuation()
    {
        // Arrange
        var text = "Hello, world! This is a test.";
        var expected = new List<string> { "hello", "world", "this", "is", "test" };

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Tokenize_ShouldConvertAllTextToLowercase_WhenTextHasMixedCase()
    {
        // Arrange
        var text = "Hello WORLD Mixed Case";
        var expected = new List<string> { "hello", "world", "mixed", "case" };

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Tokenize_ShouldFilterOutSingleCharacterTokens_WhenTextContainsSingleLetters()
    {
        // Arrange
        var text = "a b c hello world x y z";
        var expected = new List<string> { "hello", "world" };

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Tokenize_ShouldReturnEmptyEnumerable_WhenTextIsNull()
    {
        // Arrange
        string text = null;

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldReturnEmptyEnumerable_WhenTextIsWhitespace()
    {
        // Arrange
        var text = "   ";

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldReturnEmptyEnumerable_WhenTextIsEmpty()
    {
        // Arrange
        var text = "";

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldHandleMultipleSpacesAndPunctuation_WhenTextHasComplexFormatting()
    {
        // Arrange
        var text = "Hello,   world!!  This...   is   a   test!!!";
        var expected = new List<string> { "hello", "world", "this", "is", "test" };

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Tokenize_ShouldSplitOnVariousNonWordCharacters_WhenTextHasDifferentSeparators()
    {
        // Arrange
        var text = "word1,word2;word3.word4!word5?word6:word7";
        var expected = new List<string> { "word1", "word2", "word3", "word4", "word5", "word6", "word7" };

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Tokenize_ShouldFilterOutEmptyTokens_WhenTextStartsOrEndsWithNonWordCharacters()
    {
        // Arrange
        var text = "...hello world...";
        var expected = new List<string> { "hello", "world" };

        // Act
        var result = _sut.Tokenize(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}