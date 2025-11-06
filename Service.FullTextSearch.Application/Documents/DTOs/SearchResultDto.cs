namespace Service.FullTextSearch.Application.Documents.DTOs;

public record SearchResultDto(
    List<DocumentResultDto> Documents,
    int TotalResults,
    string Query);