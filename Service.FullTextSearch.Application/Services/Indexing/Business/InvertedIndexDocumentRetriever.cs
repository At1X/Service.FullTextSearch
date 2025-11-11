using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Indexing.Business;

public class InvertedIndexDocumentRetriever : IInvertedIndexDocumentRetriever
{
    public int GetFrequency(InvertedIndex instance, Guid documentId)
    {
        return instance.DocumentFrequency.GetValueOrDefault(documentId, 0);
    }

    public IReadOnlyCollection<Guid> GetDocumentIds(InvertedIndex instance)
    {
        return instance.DocumentFrequency.Keys.ToList().AsReadOnly();
    }
}