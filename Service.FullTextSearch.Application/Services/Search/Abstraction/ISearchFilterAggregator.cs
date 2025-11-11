using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Abstraction;

public interface ISearchFilterAggregator
{
    IReadOnlyCollection<ScoredDocument> Filter(
        IReadOnlyCollection<ScoredDocument> documents,
        ParsedQuery query);
}