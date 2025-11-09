using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.EntityBuilder;

public class InvertedIndexBuilder
{
    private string _term;
    private Dictionary<Guid, int>? _documentFrequency;
    
    public InvertedIndexBuilder WithTerm(string term)
    {
        _term = term;
        return this;
    }
    
    public InvertedIndexBuilder WithDocumentFrequency(Dictionary<Guid, int> documentFrequency)
    {
        _documentFrequency = documentFrequency;
        return this;
    }
    
    public InvertedIndex Build()
    {
        if (string.IsNullOrWhiteSpace(_term))
            throw new ArgumentException("Term cannot be empty", nameof(_term));
        
        return new InvertedIndex
        {
            Id = Guid.NewGuid(),
            Term = _term.ToLowerInvariant(),
            DocumentFrequency = _documentFrequency ?? new Dictionary<Guid, int>()
        };
    }
}