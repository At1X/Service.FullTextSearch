using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.SearchScorer.Abstraction;

public interface ISearchScoreCalculator
{
    IReadOnlyCollection<ScoredDocument> CalculateScore(IReadOnlyCollection<InvertedIndex> indices);
}