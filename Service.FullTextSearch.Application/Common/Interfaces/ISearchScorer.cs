using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchScorer
{
    IReadOnlyCollection<ScoredDocument> Score(IReadOnlyCollection<InvertedIndex> indices);
}