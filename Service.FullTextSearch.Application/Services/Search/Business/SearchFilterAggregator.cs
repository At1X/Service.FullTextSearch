using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Business;

public class SearchFilterAggregator : ISearchFilterAggregator
{
    private readonly IEnumerable<ITokenHandler> _handlers;

    public SearchFilterAggregator(IEnumerable<ITokenHandler> handlers)
    {
        _handlers = handlers;
    }

    public IReadOnlyCollection<ScoredDocument> Filter(
        IReadOnlyCollection<ScoredDocument> documents,
        ParsedQuery query)
    {
        return _handlers.Aggregate(
            documents, 
            (current, handler) => handler.FilterHandler(current, query)
        );
    }
}