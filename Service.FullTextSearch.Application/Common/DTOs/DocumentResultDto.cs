namespace Service.FullTextSearch.Application.Common.DTOs;

public record DocumentResultDto(
    Guid Id,
    string Title,
    string Content,
    int Score);