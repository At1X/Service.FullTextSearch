using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchScorer
{
    IEnumerable<ScoredDocument> Score(IEnumerable<InvertedIndex> indices);
}