using Service.FullTextSearch.Application.Common.Builders;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;

namespace Service.FullTextSearch.Application.Services.Indexing.Business;

public class InvertedIndexService : IDocumentIndexer
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentUpdater _invertedIndexDocumentUpdater;

    public InvertedIndexService(IInvertedIndexRepository indexRepository,  IInvertedIndexDocumentUpdater invertedIndexDocumentUpdater)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _invertedIndexDocumentUpdater =  invertedIndexDocumentUpdater ?? throw new ArgumentNullException(nameof(invertedIndexDocumentUpdater));
    }

    public void IndexTerms(Guid documentId, IDictionary<string, int> termFrequencies)
    {
        foreach (var (term, frequency) in termFrequencies)
        {
            var index = _indexRepository.GetByTerm(term) ?? new InvertedIndexBuilder().WithTerm(term).Build();
            
            _invertedIndexDocumentUpdater.AddOrUpdateDocument(index, documentId, frequency);
            
            if (_indexRepository.GetByTerm(term) == null)
                _indexRepository.Add(index);
            else
                _indexRepository.Update(index);
        }
    }
}