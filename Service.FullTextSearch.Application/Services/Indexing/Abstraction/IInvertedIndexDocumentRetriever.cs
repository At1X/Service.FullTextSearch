using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Indexing.Abstraction;

public interface IInvertedIndexDocumentRetriever
{
    int GetFrequency(InvertedIndex instance, Guid documentId);
    IReadOnlyCollection<Guid> GetDocumentIds(InvertedIndex instance);
}