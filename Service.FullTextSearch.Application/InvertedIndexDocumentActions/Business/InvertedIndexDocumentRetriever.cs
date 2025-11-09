using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.InvertedIndexDocumentActions.Business;

public class InvertedIndexDocumentRetriever : IInvertedIndexDocumentRetriever
{
    public int GetFrequency(InvertedIndex instance, Guid documentId)
    {
        return instance.DocumentFrequency.TryGetValue(documentId, out var frequency) ? frequency : 0;
    }

    public IReadOnlyCollection<Guid> GetDocumentIds(InvertedIndex instance)
    {
        return instance.DocumentFrequency.Keys.ToList().AsReadOnly();
    }
}