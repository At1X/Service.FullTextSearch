using Service.FullTextSearch.Application.Documents.Models;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchScorer
{
    IEnumerable<ScoredDocument> Score(IEnumerable<InvertedIndex> indices);
}