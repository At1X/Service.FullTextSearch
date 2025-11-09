using MediatR;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Application.SearchQueryPipeline.Abstraction;
using Service.FullTextSearch.Application.SearchResultBuilder.Abstraction;

namespace Service.FullTextSearch.Application.Documents.Queries;

public class SearchTermQueryHandler : IRequestHandler<SearchTermQuery, Result<SearchResultDto>>
{
    private readonly ISearchPipeline _searchPipeline;
    private readonly ISearchResultBuilder _resultBuilder;

    public SearchTermQueryHandler(
        ISearchPipeline searchPipeline,
        ISearchResultBuilder resultBuilder)
    {
        _searchPipeline = searchPipeline ??  throw new ArgumentNullException(nameof(searchPipeline));
        _resultBuilder = resultBuilder ?? throw new ArgumentNullException(nameof(resultBuilder));
    }

    public async Task<Result<SearchResultDto>> Handle(
        SearchTermQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var scoredDocuments = _searchPipeline
                .Search(request.SearchText);

            var results = _resultBuilder.BuildResults(scoredDocuments);

            return Result<SearchResultDto>.Success(new SearchResultDto(
                results,
                results.Count,
                request.SearchText));
        }
        catch (Exception ex)
        {
            return Result<SearchResultDto>.Failure($"Search failed: {ex.Message}");
        }
    }
}