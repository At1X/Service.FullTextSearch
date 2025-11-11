using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Indexing.Business;

public class InvertedIndexDocumentUpdater : IInvertedIndexDocumentUpdater
{
    public void AddOrUpdateDocument(InvertedIndex instance, Guid documentId, int frequency)
    {
        if (frequency <= 0)
            throw new ArgumentException("Frequency must be positive", nameof(frequency));

        instance.DocumentFrequency[documentId] = frequency;
    }
}