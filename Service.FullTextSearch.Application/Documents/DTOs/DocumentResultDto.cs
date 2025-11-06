namespace Service.FullTextSearch.Application.Documents.DTOs;

public record DocumentResultDto(
    Guid Id,
    string Title,
    string Content,
    int Score);