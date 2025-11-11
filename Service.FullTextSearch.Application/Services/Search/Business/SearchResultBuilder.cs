using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Business;

public class SearchResultBuilder : ISearchResultBuilder
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ISearchResultMapper _resultMapper;

    public SearchResultBuilder(
        IDocumentRepository documentRepository,
        ISearchResultMapper resultMapper)
    {
        _documentRepository = documentRepository;
        _resultMapper = resultMapper;
    }

    public IReadOnlyCollection<DocumentResultDto> BuildResults(
        IReadOnlyCollection<ScoredDocument> scoredDocuments)
    {
        return scoredDocuments
            .Select(sd => (Document: _documentRepository.GetById(sd.DocumentId), sd.Score))
            .Select(tuple => _resultMapper.MapToDto(tuple.Document!, tuple.Score))
            .ToList();
    }
}