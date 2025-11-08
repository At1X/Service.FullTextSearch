namespace Service.FullTextSearch.Domain.Entities;

public class InvertedIndex
{
    public Guid Id { get; private set; }
    public string Term { get; private set; }
    public Dictionary<Guid, int> DocumentFrequency { get; set; }
    
    public InvertedIndex(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Term cannot be empty", nameof(term));
        Id  = Guid.NewGuid();
        Term = term.ToLowerInvariant();
        DocumentFrequency = new Dictionary<Guid, int>();
    }
}