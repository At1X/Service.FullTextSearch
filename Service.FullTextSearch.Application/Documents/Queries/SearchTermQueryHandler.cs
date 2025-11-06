using MediatR;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Interfaces;

namespace Service.FullTextSearch.Application.Documents.Queries;

public class SearchTermQueryHandler : IRequestHandler<SearchTermQuery, Result<SearchResultDto>>
{
    private readonly ISearchPipeline _searchPipeline;
    private readonly IDocumentRepository _documentRepository;
    private readonly ISearchResultMapper _resultMapper;

    public SearchTermQueryHandler(
        ISearchPipeline searchPipeline,
        IDocumentRepository documentRepository,
        ISearchResultMapper resultMapper)
    {
        _searchPipeline = searchPipeline ??  throw new ArgumentNullException(nameof(searchPipeline));
        _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _resultMapper = resultMapper ??  throw new ArgumentNullException(nameof(resultMapper));
    }

    public async Task<Result<SearchResultDto>> Handle(
        SearchTermQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var scoredDocuments = _searchPipeline
                .Search(request.SearchText);

            var results = scoredDocuments
                .Select(sd => _documentRepository.GetById(sd.DocumentId))
                .Where(doc => doc != null)
                .Zip(scoredDocuments, (doc, sd) => _resultMapper.MapToDto(doc, sd.Score))
                .ToList();

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