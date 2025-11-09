namespace Service.FullTextSearch.Application.Documents.DTOs;

public record SearchResultDto(
    IReadOnlyCollection<DocumentResultDto> Documents,
    int TotalResults,
    string Query);