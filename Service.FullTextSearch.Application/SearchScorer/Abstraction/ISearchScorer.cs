using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.SearchScorer.Abstraction;

public interface ISearchScorer
{
    IReadOnlyCollection<ScoredDocument> Score(IReadOnlyCollection<InvertedIndex> indices);
}