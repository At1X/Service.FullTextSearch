using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ISearchResultMapper
{
    DocumentResultDto MapToDto(Document document, int score);
}