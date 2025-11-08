using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Models;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchResultBuilder
{
    List<DocumentResultDto> BuildResults(
        IEnumerable<ScoredDocument> scoredDocuments);
}