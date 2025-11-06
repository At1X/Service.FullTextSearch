using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Interfaces;

namespace Service.FullTextSearch.Infrastructure.Persistence.Repositories;

public class InMemoryInvertedIndexRepository : IInvertedIndexRepository
{
    public Task<InvertedIndex?> GetByTermAsync(string term, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<InvertedIndex>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<InvertedIndex> AddAsync(InvertedIndex index, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(InvertedIndex index, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string term, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<InvertedIndex>> SearchTermsAsync(IEnumerable<string> terms, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}