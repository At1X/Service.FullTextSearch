
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Abstraction;

public interface ISearchPipeline
{
    IReadOnlyCollection<ScoredDocument> Search(
        string searchText);
}