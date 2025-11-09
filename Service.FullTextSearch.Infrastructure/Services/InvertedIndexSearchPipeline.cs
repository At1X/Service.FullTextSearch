using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Infrastructure.Services;

public class InvertedIndexSearchPipeline : ISearchPipeline
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly ITextProcessor _textProcessor;
    private readonly ISearchScorer _scorer;

    public InvertedIndexSearchPipeline(IInvertedIndexRepository indexRepository, ITextProcessor textProcessor,
        ISearchScorer scorer)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _textProcessor = textProcessor ?? throw new ArgumentNullException(nameof(textProcessor));
        _scorer = scorer ??  throw new ArgumentNullException(nameof(scorer));
    }

    public IReadOnlyCollection<ScoredDocument> Search(
        string searchText)
    {
        var tokens = _textProcessor.Process(searchText);
        var indices = _indexRepository.SearchTerms(tokens);
        return _scorer.Score(indices);
    }
}