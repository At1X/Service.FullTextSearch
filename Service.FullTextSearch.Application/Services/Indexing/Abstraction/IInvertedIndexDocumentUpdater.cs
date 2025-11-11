using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Indexing.Abstraction;

public interface IInvertedIndexDocumentUpdater
{
    void AddOrUpdateDocument(InvertedIndex instance, Guid documentId, int frequency);
}