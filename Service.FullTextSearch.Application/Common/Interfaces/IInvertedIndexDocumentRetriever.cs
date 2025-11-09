using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IInvertedIndexDocumentRetriever
{
    int GetFrequency(InvertedIndex instance, Guid documentId);
    IReadOnlyCollection<Guid> GetDocumentIds(InvertedIndex instance);
}