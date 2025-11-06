using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Domain.Interfaces;

public interface IInvertedIndexRepository
{
    InvertedIndex? GetByTerm(string term);
    IReadOnlyList<InvertedIndex> GetAll();
    InvertedIndex Add(InvertedIndex index);
    void Update(InvertedIndex index);
    void Delete(string term);
    IReadOnlyList<InvertedIndex> SearchTerms(IEnumerable<string> terms);
    Dictionary<string, InvertedIndex> GetAllIndices();
}