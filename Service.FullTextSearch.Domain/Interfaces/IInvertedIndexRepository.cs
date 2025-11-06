using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Domain.Interfaces;

public interface IInvertedIndexRepository
{
    Task<InvertedIndex?> GetByTermAsync(string term, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvertedIndex>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InvertedIndex> AddAsync(InvertedIndex index, CancellationToken cancellationToken = default);
    Task UpdateAsync(InvertedIndex index, CancellationToken cancellationToken = default);
    Task DeleteAsync(string term, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvertedIndex>> SearchTermsAsync(IEnumerable<string> terms, CancellationToken cancellationToken = default);
}