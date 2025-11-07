using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Interfaces;

namespace Service.FullTextSearch.Infrastructure.Services;

public class InvertedIndexService : IDocumentIndexer
{
    private readonly IInvertedIndexRepository _indexRepository;

    public InvertedIndexService(IInvertedIndexRepository indexRepository)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
    }

    public void IndexTerms(Guid documentId, IDictionary<string, int> termFrequencies)
    {
        foreach (var (term, frequency) in termFrequencies)
        {
            var index = _indexRepository.GetByTerm(term) ?? new InvertedIndex(term);
            
            index.AddOrUpdateDocument(documentId, frequency);
            
            if (_indexRepository.GetByTerm(term) == null)
                _indexRepository.Add(index);
            else
                _indexRepository.Update(index);
        }
    }
}