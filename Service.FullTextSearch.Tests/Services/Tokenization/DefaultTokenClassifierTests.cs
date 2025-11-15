using FluentAssertions;
using Service.FullTextSearch.Application.Services.Tokenization.Business;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Tests.Services.Tokenization;

public class DefaultTokenClassifierTests
{
    private readonly DefaultTokenClassifier _sut;

    public DefaultTokenClassifierTests()
    {
        _sut = new DefaultTokenClassifier();
    }

    [Fact]
    public void Classify_ShouldReturnOptionalClassification_WhenTokenStartsWithPlus()
    {
        // Arrange
        var token = "+required";

        // Act
        var result = _sut.Classify(token);

        // Assert
        result.Type.Should().Be(TokenType.Optional);
        result.Term.Should().Be("required");
    }

    [Fact]
    public void Classify_ShouldReturnExcludedClassification_WhenTokenStartsWithMinus()
    {
        // Arrange
        var token = "-excluded";

        // Act
        var result = _sut.Classify(token);

        // Assert
        result.Type.Should().Be(TokenType.Excluded);
        result.Term.Should().Be("excluded");
    }

    [Fact]
    public void Classify_ShouldReturnRequiredClassification_WhenTokenStartsWithoutPrefix()
    {
        // Arrange
        var token = "regular";

        // Act
        var result = _sut.Classify(token);

        // Assert
        result.Type.Should().Be(TokenType.Required);
        result.Term.Should().Be("regular");
    }

    [Fact]
    public void Classify_ShouldHandleTokenWithMultiplePlusesAfterFirst()
    {
        // Arrange
        var token = "+term++";

        // Act
        var result = _sut.Classify(token);

        // Assert
        result.Type.Should().Be(TokenType.Optional);
        result.Term.Should().Be("term++");
    }

    [Fact]
    public void Classify_ShouldHandleTokenWithMultipleMinusesAfterFirst()
    {
        // Arrange
        var token = "-term--";

        // Act
        var result = _sut.Classify(token);

        // Assert
        result.Type.Should().Be(TokenType.Excluded);
        result.Term.Should().Be("term--");
    }

    [Theory]
    [InlineData("+hello", TokenType.Optional, "hello")]
    [InlineData("-world", TokenType.Excluded, "world")]
    [InlineData("test", TokenType.Required, "test")]
    [InlineData("+a", TokenType.Optional, "a")]
    [InlineData("-b", TokenType.Excluded, "b")]
    [InlineData("c", TokenType.Required, "c")]
    public void Classify_ShouldClassifyCorrectly_ForVariousInputs(
        string token, 
        TokenType expectedType, 
        string expectedTerm)
    {
        // Act
        var result = _sut.Classify(token);

        // Assert
        result.Type.Should().Be(expectedType);
        result.Term.Should().Be(expectedTerm);
    }
}