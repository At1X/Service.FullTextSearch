using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IInvertedIndexDocumentRetriever
{
    int GetFrequency(InvertedIndex instance, Guid documentId);
    IReadOnlyList<Guid> GetDocumentIds(InvertedIndex instance);
}