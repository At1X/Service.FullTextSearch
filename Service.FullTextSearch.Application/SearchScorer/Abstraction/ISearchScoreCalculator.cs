using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.SearchScorer.Abstraction;

public interface ISearchScoreCalculator
{
    IReadOnlyCollection<ScoredDocument> CalculateScore(IReadOnlyCollection<InvertedIndex> indices);
}