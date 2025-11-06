using Service.FullTextSearch.Domain.Common;

namespace Service.FullTextSearch.Domain.Entities;

public class InvertedIndex : BaseEntity
{
    public string Term { get; private set; }
    public Dictionary<Guid, int> DocumentFrequency { get; private set; }

    private InvertedIndex() 
    { 
        DocumentFrequency = new Dictionary<Guid, int>();
    }

    public InvertedIndex(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Term cannot be empty", nameof(term));

        Term = term.ToLowerInvariant();
        DocumentFrequency = new Dictionary<Guid, int>();
    }

    public void AddOrUpdateDocument(Guid documentId, int frequency)
    {
        if (frequency <= 0)
            throw new ArgumentException("Frequency must be positive", nameof(frequency));

        if (DocumentFrequency.ContainsKey(documentId))
            DocumentFrequency[documentId] = frequency;
        else
            DocumentFrequency.Add(documentId, frequency);

        SetUpdated();
    }

    public void RemoveDocument(Guid documentId)
    {
        if (DocumentFrequency.ContainsKey(documentId))
        {
            DocumentFrequency.Remove(documentId);
            SetUpdated();
        }
    }

    public int GetFrequency(Guid documentId)
    {
        return DocumentFrequency.TryGetValue(documentId, out var frequency) ? frequency : 0;
    }

    public IReadOnlyList<Guid> GetDocumentIds()
    {
        return DocumentFrequency.Keys.ToList().AsReadOnly();
    }
}