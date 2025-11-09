using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.StopWordRemover.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Business;
using Service.FullTextSearch.Application.Tokenizer.Abstraction;

namespace Service.FullTextSearch.Tests.Services;

public class StandardTextProcessorTests
{
    private readonly ITokenizer _tokenizer;
    private readonly IStopWordRemover _stopWordRemover;
    private readonly ITextProcessor _sut;

    public StandardTextProcessorTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _stopWordRemover = Substitute.For<IStopWordRemover>();
        _sut = new StandardTextProcessor(_tokenizer, _stopWordRemover);
    }

    [Fact]
    public void Process_ShouldReturnLowercaseTerms_WhenValidTextProvided()
    {
        // Arrange
        var text = "The Quick Brown Fox";
        var tokens = new[] { "The", "Quick", "Brown", "Fox" };
        var cleaned = new[] { "Quick", "Brown", "Fox" };
        var expected = new[] { "quick", "brown", "fox" };

        _tokenizer.Tokenize(text).Returns(tokens);
        _stopWordRemover.RemoveStopWords(tokens).Returns(cleaned);

        // Act
        var result = _sut.Process(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Process_ShouldCallTokenizerWithCorrectText_WhenMethodCalled()
    {
        // Arrange
        var text = "Test input text";
        _tokenizer.Tokenize(text).Returns(Array.Empty<string>());
        _stopWordRemover.RemoveStopWords(Arg.Any<IReadOnlyCollection<string>>()).Returns(Array.Empty<string>());

        // Act
        _sut.Process(text);

        // Assert
        _tokenizer.Received(1).Tokenize(text);
    }

    [Fact]
    public void Process_ShouldCallStopWordRemoverWithTokenizerOutput_WhenMethodCalled()
    {
        // Arrange
        var text = "Test input text";
        var tokens = new[] { "Test", "input", "text" };
        _tokenizer.Tokenize(text).Returns(tokens);
        _stopWordRemover.RemoveStopWords(tokens).Returns(Array.Empty<string>());

        // Act
        _sut.Process(text);

        // Assert
        _stopWordRemover.Received(1).RemoveStopWords(tokens);
    }

    [Fact]
    public void CalculateTermFrequency_ShouldReturnCorrectFrequency_WhenMultipleSameTerms()
    {
        // Arrange
        var text = "hello world hello test";
        var tokens = new[] { "hello", "world", "hello", "test" };
        var cleaned = new[] { "hello", "world", "hello", "test" };
        var expected = new Dictionary<string, int>
        {
            { "hello", 2 },
            { "world", 1 },
            { "test", 1 }
        };

        _tokenizer.Tokenize(text).Returns(tokens);
        _stopWordRemover.RemoveStopWords(tokens).Returns(cleaned);

        // Act
        var result = _sut.CalculateTermFrequency(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateTermFrequency_ShouldReturnEmptyDictionary_WhenNoTerms()
    {
        // Arrange
        var text = "";
        _tokenizer.Tokenize(text).Returns(Array.Empty<string>());
        _stopWordRemover.RemoveStopWords(Arg.Any<IReadOnlyCollection<string>>()).Returns(Array.Empty<string>());

        // Act
        var result = _sut.CalculateTermFrequency(text);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateTermFrequency_ShouldApplyLowerCaseToAllTerms_WhenMixedCaseTerms()
    {
        // Arrange
        var text = "Hello HELLO hello";
        var tokens = new[] { "Hello", "HELLO", "hello" };
        var cleaned = new[] { "Hello", "HELLO", "hello" };
        var expected = new Dictionary<string, int> { { "hello", 3 } };

        _tokenizer.Tokenize(text).Returns(tokens);
        _stopWordRemover.RemoveStopWords(tokens).Returns(cleaned);

        // Act
        var result = _sut.CalculateTermFrequency(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CalculateTermFrequency_ShouldRemoveStopWordsBeforeCounting_WhenStopWordsPresent()
    {
        // Arrange
        var text = "the quick brown fox";
        var tokens = new[] { "the", "quick", "brown", "fox" };
        var cleaned = new[] { "quick", "brown", "fox" };
        var expected = new Dictionary<string, int>
        {
            { "quick", 1 },
            { "brown", 1 },
            { "fox", 1 }
        };

        _tokenizer.Tokenize(text).Returns(tokens);
        _stopWordRemover.RemoveStopWords(tokens).Returns(cleaned);

        // Act
        var result = _sut.CalculateTermFrequency(text);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
}