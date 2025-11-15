namespace Service.FullTextSearch.Application.Common.DTOs;

public record SearchResultDto(
    IReadOnlyCollection<DocumentResultDto> Documents,
    int TotalResults,
    string Query);