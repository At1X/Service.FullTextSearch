
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchPipeline
{
    IEnumerable<ScoredDocument> Search(
        string searchText);
}