using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.SearchResultMapper.Abstraction;

public interface ISearchResultMapper
{
    DocumentResultDto MapToDto(Document document, int score);
}