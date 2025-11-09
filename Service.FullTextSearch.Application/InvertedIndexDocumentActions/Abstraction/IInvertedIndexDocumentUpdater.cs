using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.InvertedIndexDocumentActions.Abstraction;

public interface IInvertedIndexDocumentUpdater
{
    void AddOrUpdateDocument(InvertedIndex instance, Guid documentId, int frequency);
}