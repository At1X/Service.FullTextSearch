using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IInvertedIndexRepository
{
    InvertedIndex? GetByTerm(string term);
    IReadOnlyCollection<InvertedIndex> GetAll();
    InvertedIndex Add(InvertedIndex index);
    void Update(InvertedIndex index);
    void Delete(string term);
    IReadOnlyCollection<InvertedIndex> SearchTerms(IReadOnlyCollection<string> terms);
    IDictionary<string, InvertedIndex> GetAllIndices();
}