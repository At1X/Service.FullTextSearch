using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Domain.Entities;

public record TokenClassification(TokenType Type, string Term);