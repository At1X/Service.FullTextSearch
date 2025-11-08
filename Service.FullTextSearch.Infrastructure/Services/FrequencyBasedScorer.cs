using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.Models;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Infrastructure.Services;

public class FrequencyBasedScorer : ISearchScorer
{
    private readonly IInvertedIndexDocumentRetriever _invertedIndexDocumentRetriever;

    public FrequencyBasedScorer(IInvertedIndexDocumentRetriever invertedIndexDocumentRetriever)
    {
        _invertedIndexDocumentRetriever = invertedIndexDocumentRetriever ??  throw new ArgumentNullException(nameof(invertedIndexDocumentRetriever));
    }
    public IEnumerable<ScoredDocument> Score(IEnumerable<InvertedIndex> indices)
    {
        var documentScores = new Dictionary<Guid, int>();
        
        foreach (var index in indices)
        {
            foreach (var docId in _invertedIndexDocumentRetriever.GetDocumentIds(index))
            {
                var frequency = _invertedIndexDocumentRetriever.GetFrequency(index, docId);
                documentScores[docId] = documentScores.GetValueOrDefault(docId) + frequency;
            }
        }
        
        return documentScores
            .OrderByDescending(x => x.Value)
            .Select(x => new ScoredDocument(x.Key, x.Value));
    }
}