using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Abstraction;

public interface ISearchScoreCalculator
{
    IReadOnlyCollection<ScoredDocument> CalculateScore(IReadOnlyCollection<InvertedIndex> indices);
}