
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Abstraction;

public interface ISearcher
{
    IReadOnlyCollection<ScoredDocument> Search(
        string searchText);
}