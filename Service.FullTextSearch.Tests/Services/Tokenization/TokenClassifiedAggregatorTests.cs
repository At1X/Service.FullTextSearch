using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Business;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Tests.Services.Tokenization;

public class TokenClassifiedAggregatorTests
{
    private readonly ITokenHandler _requiredHandler;
    private readonly ITokenHandler _optionalHandler;
    private readonly ITokenHandler _excludedHandler;
    private readonly TokenClassifiedAggregator _sut;

    public TokenClassifiedAggregatorTests()
    {
        _requiredHandler = Substitute.For<ITokenHandler>();
        _optionalHandler = Substitute.For<ITokenHandler>();
        _excludedHandler = Substitute.For<ITokenHandler>();

        _requiredHandler.CanHandle(TokenType.Required).Returns(true);
        _requiredHandler.CanHandle(TokenType.Optional).Returns(false);
        _requiredHandler.CanHandle(TokenType.Excluded).Returns(false);

        _optionalHandler.CanHandle(TokenType.Required).Returns(false);
        _optionalHandler.CanHandle(TokenType.Optional).Returns(true);
        _optionalHandler.CanHandle(TokenType.Excluded).Returns(false);

        _excludedHandler.CanHandle(TokenType.Required).Returns(false);
        _excludedHandler.CanHandle(TokenType.Optional).Returns(false);
        _excludedHandler.CanHandle(TokenType.Excluded).Returns(true);

        var handlers = new List<ITokenHandler> { _requiredHandler, _optionalHandler, _excludedHandler };
        _sut = new TokenClassifiedAggregator(handlers);
    }

    [Fact]
    public void Aggregate_ShouldReturnParsedQuery_WhenClassificationsProvided()
    {
        // Arrange
        var classifications = new List<TokenClassification>
        {
            new TokenClassification(TokenType.Required, "term1"),
            new TokenClassification(TokenType.Optional, "term2"),
            new TokenClassification(TokenType.Excluded, "term3")
        };

        // Act
        var result = _sut.Aggregate(classifications);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ParsedQuery>();
        _requiredHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "term1");
        _optionalHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "term2");
        _excludedHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "term3");
    }
    

    [Fact]
    public void Aggregate_ShouldCallRequiredHandler_WhenRequiredClassificationProvided()
    {
        // Arrange
        var classifications = new List<TokenClassification>
        {
            new TokenClassification(TokenType.Required, "required")
        };

        // Act
        var result = _sut.Aggregate(classifications);

        // Assert
        _requiredHandler.Received(1).CanHandle(TokenType.Required);
        _requiredHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "required");
        _optionalHandler.DidNotReceive().HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
        _excludedHandler.DidNotReceive().HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
    }

    [Fact]
    public void Aggregate_ShouldCallOptionalHandler_WhenOptionalClassificationProvided()
    {
        // Arrange
        var classifications = new List<TokenClassification>
        {
            new TokenClassification(TokenType.Optional, "optional")
        };

        // Act
        var result = _sut.Aggregate(classifications);

        // Assert
        _optionalHandler.Received(1).CanHandle(TokenType.Optional);
        _optionalHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "optional");
        _requiredHandler.DidNotReceive().HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
        _excludedHandler.DidNotReceive().HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
    }

    [Fact]
    public void Aggregate_ShouldCallExcludedHandler_WhenExcludedClassificationProvided()
    {
        // Arrange
        var classifications = new List<TokenClassification>
        {
            new TokenClassification(TokenType.Excluded, "excluded")
        };

        // Act
        var result = _sut.Aggregate(classifications);

        // Assert
        _excludedHandler.Received(1).CanHandle(TokenType.Excluded);
        _excludedHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "excluded");
        _requiredHandler.DidNotReceive().HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
        _optionalHandler.DidNotReceive().HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
    }

    [Fact]
    public void Aggregate_ShouldHandleMultipleClassificationsOfSameType()
    {
        // Arrange
        var classifications = new List<TokenClassification>
        {
            new TokenClassification(TokenType.Required, "term1"),
            new TokenClassification(TokenType.Required, "term2"),
            new TokenClassification(TokenType.Required, "term3")
        };

        // Act
        var result = _sut.Aggregate(classifications);

        // Assert
        _requiredHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "term1");
        _requiredHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "term2");
        _requiredHandler.Received(1).HandleParser(Arg.Any<ParsedQuery>(), "term3");
        _requiredHandler.Received(3).HandleParser(Arg.Any<ParsedQuery>(), Arg.Any<string>());
    }
    
}