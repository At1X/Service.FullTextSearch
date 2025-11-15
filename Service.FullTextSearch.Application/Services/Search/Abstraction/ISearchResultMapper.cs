using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Abstraction;

public interface ISearchResultMapper
{
    DocumentResultDto MapToDto(Document document, int score);
}