using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Business;

public class AdvancedInvertedIndexSearchPipeline : ISearchPipeline
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IQueryParser _queryParser;
    private readonly ISearchScoreCalculator _scoreCalculator;
    private readonly ISearchFilterAggregator _searchFilterAggregator;

    public AdvancedInvertedIndexSearchPipeline(
        IInvertedIndexRepository indexRepository,
        IQueryParser queryParser,
        ISearchScoreCalculator scoreCalculator,
        ISearchFilterAggregator searchFilterAggregator)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _queryParser = queryParser ?? throw new ArgumentNullException(nameof(queryParser));
        _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        _searchFilterAggregator = searchFilterAggregator ?? throw new ArgumentNullException(nameof(searchFilterAggregator));
    }

    public IReadOnlyCollection<ScoredDocument> Search(string searchText)
    {
        var parsedQuery = _queryParser.Parse(searchText);

        var allTerms = parsedQuery.RequiredTerms
            .Concat(parsedQuery.OptionalTerms)
            .Distinct()
            .ToList();

        if (!allTerms.Any())
        {
            return Array.Empty<ScoredDocument>();
        }

        var indices = _indexRepository.SearchTerms(allTerms);
        var scoredDocuments = _scoreCalculator.CalculateScore(indices);

        var filteredDocuments = _searchFilterAggregator.Filter(scoredDocuments, parsedQuery);

        return filteredDocuments;
    }
}