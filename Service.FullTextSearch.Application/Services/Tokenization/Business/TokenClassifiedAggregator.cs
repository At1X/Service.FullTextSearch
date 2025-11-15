using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Tokenization.Business;

public class TokenClassifiedAggregator : ITokenClassifiedAggregator
{
    private readonly IEnumerable<ITokenHandler> _handlers;

    public TokenClassifiedAggregator(IEnumerable<ITokenHandler> handlers)
    {
        _handlers = handlers;
    }

    public ParsedQuery Aggregate(IReadOnlyCollection<TokenClassification> classifications)
    {
        var parsedQuery = new ParsedQuery();

        foreach (var classification in classifications)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(classification.Type));
            handler?.HandleParser(parsedQuery, classification.Term);
        }

        return parsedQuery;
    }
}