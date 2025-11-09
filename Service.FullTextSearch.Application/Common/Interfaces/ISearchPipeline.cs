
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchPipeline
{
    IReadOnlyCollection<ScoredDocument> Search(
        string searchText);
}