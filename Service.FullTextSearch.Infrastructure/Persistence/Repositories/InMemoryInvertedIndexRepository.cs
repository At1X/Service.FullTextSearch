using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Infrastructure.Persistence.Repositories;

public class InMemoryInvertedIndexRepository : IInvertedIndexRepository
{
    private readonly Dictionary<string, InvertedIndex> _indices = new();

    public InvertedIndex? GetByTerm(string term)
    {
        _indices.TryGetValue(term.ToLowerInvariant(), out var index);
        return index;
    }

    public IReadOnlyList<InvertedIndex> GetAll()
    {
        return _indices.Values.ToList();
    }

    public InvertedIndex Add(InvertedIndex index)
    {
        _indices[index.Term] = index;
        return index;
    }

    public void Update(InvertedIndex index)
    {
        _indices[index.Term] = index;
    }

    public void Delete(string term)
    {
        _indices.Remove(term.ToLowerInvariant());
    }

    public IReadOnlyList<InvertedIndex> SearchTerms(
        IEnumerable<string> terms)
    {
        var results = terms
            .Select(term => _indices.TryGetValue(term.ToLowerInvariant(), out var index) ? index : null)
            .Where(i => i != null)
            .Cast<InvertedIndex>()
            .ToList();

        return results;
    }

    public Dictionary<string, InvertedIndex> GetAllIndices()
    {
        return _indices.ToDictionary(i => i.Key, i => i.Value);
    }
}