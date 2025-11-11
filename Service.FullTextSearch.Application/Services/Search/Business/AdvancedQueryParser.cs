using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Business;

public class AdvancedQueryParser : IQueryParser
{
    private readonly ITokenizer _tokenizer;
    private readonly ITokenClassifier _tokenClassifier;
    private readonly ITokenClassifiedAggregator _tokenClassifiedAggregator;

    public AdvancedQueryParser(
        ITokenizer tokenizer,
        ITokenClassifier tokenClassifier,
        ITokenClassifiedAggregator tokenClassifiedAggregator)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _tokenClassifier = tokenClassifier ?? throw new ArgumentNullException(nameof(tokenClassifier));
        _tokenClassifiedAggregator = tokenClassifiedAggregator ??
                                     throw new ArgumentNullException(nameof(tokenClassifiedAggregator));
    }

    public ParsedQuery Parse(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new ParsedQuery();
        }

        var tokens = _tokenizer.Tokenize(query);
        var classifications = tokens
            .Select(token => _tokenClassifier.Classify(token))
            .ToList();

        return _tokenClassifiedAggregator.Aggregate(classifications);
    }
}