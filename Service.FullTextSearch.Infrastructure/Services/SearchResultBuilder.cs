using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Tests.Services;

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

    public List<DocumentResultDto> BuildResults(
        IEnumerable<ScoredDocument> scoredDocuments)
    {
        return scoredDocuments
            .Select(sd =>
            {
                var doc = _documentRepository.GetById(sd.DocumentId);
                return doc != null ? _resultMapper.MapToDto(doc, sd.Score) : null;
            })
            .Where(result => result != null)
            .ToList();
    }
}