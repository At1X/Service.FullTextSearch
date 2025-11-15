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

    public IReadOnlyCollection<InvertedIndex> GetAll()
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

    public IReadOnlyCollection<InvertedIndex> SearchTerms(
        IReadOnlyCollection<string> terms)
    {
        return terms
            .Select(term => term.ToLowerInvariant())
            .Where(_indices.ContainsKey)
            .Select(term => _indices[term])
            .ToList();
    }

    public IDictionary<string, InvertedIndex> GetAllIndices()
    {
        return _indices.ToDictionary(i => i.Key, i => i.Value);
    }
}