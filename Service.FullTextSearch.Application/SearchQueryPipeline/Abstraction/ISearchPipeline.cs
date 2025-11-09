
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.SearchQueryPipeline.Abstraction;

public interface ISearchPipeline
{
    IReadOnlyCollection<ScoredDocument> Search(
        string searchText);
}