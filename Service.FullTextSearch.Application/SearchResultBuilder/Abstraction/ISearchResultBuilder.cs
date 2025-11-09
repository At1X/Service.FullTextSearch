using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.SearchResultBuilder.Abstraction;

public interface ISearchResultBuilder
{
    IReadOnlyCollection<DocumentResultDto> BuildResults(
        IReadOnlyCollection<ScoredDocument> scoredDocuments);
}