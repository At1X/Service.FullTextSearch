using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Infrastructure.Services;

public class InvertedIndexDocumentUpdater : IInvertedIndexDocumentUpdater
{
    public void AddOrUpdateDocument(InvertedIndex instance, Guid documentId, int frequency)
    {
        if (frequency <= 0)
            throw new ArgumentException("Frequency must be positive", nameof(frequency));

        instance.DocumentFrequency[documentId] = frequency;
    }
}