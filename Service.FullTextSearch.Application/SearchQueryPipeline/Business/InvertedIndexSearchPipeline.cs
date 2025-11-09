using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.SearchQueryPipeline.Abstraction;
using Service.FullTextSearch.Application.SearchScorer.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Abstraction;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.SearchQueryPipeline.Business;

public class InvertedIndexSearchPipeline : ISearchPipeline
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly ITextProcessor _textProcessor;
    private readonly ISearchScoreCalculator _scoreCalculator;

    public InvertedIndexSearchPipeline(IInvertedIndexRepository indexRepository, ITextProcessor textProcessor,
        ISearchScoreCalculator scoreCalculator)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _textProcessor = textProcessor ?? throw new ArgumentNullException(nameof(textProcessor));
        _scoreCalculator = scoreCalculator ??  throw new ArgumentNullException(nameof(scoreCalculator));
    }

    public IReadOnlyCollection<ScoredDocument> Search(
        string searchText)
    {
        var tokens = _textProcessor.PreProcessText(searchText);
        var indices = _indexRepository.SearchTerms(tokens);
        return _scoreCalculator.CalculateScore(indices);
    }
}