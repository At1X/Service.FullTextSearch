using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Application.SearchResultBuilder.Abstraction;
using Service.FullTextSearch.Application.SearchResultMapper.Abstraction;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.SearchResultBuilder.Business;

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