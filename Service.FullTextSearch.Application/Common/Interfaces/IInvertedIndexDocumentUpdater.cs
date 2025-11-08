using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IInvertedIndexDocumentUpdater
{
    void AddOrUpdateDocument(InvertedIndex instance, Guid documentId, int frequency);
}