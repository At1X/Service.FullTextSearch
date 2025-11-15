using MediatR;
using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Services.Search.Abstraction;

namespace Service.FullTextSearch.Application.Services.Mediator.Queries;

public class SearchTermQueryHandler : IRequestHandler<SearchTermQuery, Result<SearchResultDto>>
{
    private readonly ISearcher _searcher;
    private readonly ISearchResultBuilder _resultBuilder;

    public SearchTermQueryHandler(
        ISearcher searcher,
        ISearchResultBuilder resultBuilder)
    {
        _searcher = searcher ??  throw new ArgumentNullException(nameof(searcher));
        _resultBuilder = resultBuilder ?? throw new ArgumentNullException(nameof(resultBuilder));
    }

    public async Task<Result<SearchResultDto>> Handle(
        SearchTermQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var scoredDocuments = _searcher
                .Search(request.SearchText);

            var results = _resultBuilder.BuildResults(scoredDocuments);

            return Result<SearchResultDto>.Success(new SearchResultDto(
                results,
                results.Count,
                request.SearchText));
        }
        catch (Exception ex)
        {
            return Result<SearchResultDto>.Failure($"DocumentIndex failed: {ex.Message}");
        }
    }
}