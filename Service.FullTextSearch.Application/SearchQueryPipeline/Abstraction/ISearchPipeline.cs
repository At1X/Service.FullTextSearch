
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.SearchQueryPipeline.Abstraction;

public interface ISearchPipeline
{
    IReadOnlyCollection<ScoredDocument> Search(
        string searchText);
}