using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Services.Search.Business;
using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Tests.Services.Search;

public class AdvancedQueryParserTests
{
    private readonly ITextProcessor _textProcessor;
    private readonly ITokenizer _tokenizer;
    private readonly ITokenClassifier _tokenClassifier;
    private readonly ITokenClassifiedAggregator _tokenClassifiedAggregator;
    private readonly AdvancedQueryParser _sut;

    public AdvancedQueryParserTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _tokenClassifier = Substitute.For<ITokenClassifier>();
        _tokenClassifiedAggregator = Substitute.For<ITokenClassifiedAggregator>();
        _sut = new AdvancedQueryParser(
            _tokenizer,
            _tokenClassifier,
            _tokenClassifiedAggregator);
    }

    [Fact]
    public void Parse_ShouldReturnParsedQuery_WhenValidQueryProvided()
    {
        // Arrange
        var query = "required +optional -excluded";
        var tokens = new List<string> { "required", "+optional", "-excluded" };
        
        var classification1 = new TokenClassification( TokenType.Required, "required" );
        var classification2 = new TokenClassification( TokenType.Optional, "optional" );
        var classification3 = new TokenClassification( TokenType.Excluded, "excluded" );
        
        var parsedQuery = new ParsedQuery()
        {
            RequiredTerms = new List<string> { "required" },
            OptionalTerms = new List<string> { "optional" },
            ExcludedTerms = new List<string> { "excluded" }
        };

        _tokenizer.Tokenize(query).Returns(tokens);
        _tokenClassifier.Classify("required").Returns(classification1);
        _tokenClassifier.Classify("+optional").Returns(classification2);
        _tokenClassifier.Classify("-excluded").Returns(classification3);
        _tokenClassifiedAggregator.Aggregate(Arg.Any<List<TokenClassification>>()).Returns(parsedQuery);

        // Act
        var result = _sut.Parse(query);

        // Assert
        result.Should().BeEquivalentTo(parsedQuery);
        _tokenizer.Received(1).Tokenize(query);
        _tokenClassifier.Received(1).Classify("required");
        _tokenClassifier.Received(1).Classify("+optional");
        _tokenClassifier.Received(1).Classify("-excluded");
        _tokenClassifiedAggregator.Received(1).Aggregate(Arg.Is<List<TokenClassification>>(list =>
            list.Count == 3));
    }

}