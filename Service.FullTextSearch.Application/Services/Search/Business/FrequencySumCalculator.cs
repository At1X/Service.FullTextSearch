using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Business;

public class FrequencySumCalculator : ISearchScoreCalculator
{
    private readonly IInvertedIndexDocumentRetriever _invertedIndexDocumentRetriever;

    public FrequencySumCalculator(IInvertedIndexDocumentRetriever invertedIndexDocumentRetriever)
    {
        _invertedIndexDocumentRetriever = invertedIndexDocumentRetriever ??  throw new ArgumentNullException(nameof(invertedIndexDocumentRetriever));
    }
    public IReadOnlyCollection<ScoredDocument> CalculateScore(IReadOnlyCollection<InvertedIndex> indices)
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
            .Select(x => new ScoredDocument { DocumentId = x.Key, Score = x.Value })
            .ToList();
    }
}