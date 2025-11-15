using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Application.Services.Tokenization.Abstraction;

public interface ITokenHandler
{
    bool CanHandle(TokenType type);
    void HandleParser(ParsedQuery query, string term);

    IReadOnlyCollection<ScoredDocument> FilterHandler(
        IReadOnlyCollection<ScoredDocument> documents,
        ParsedQuery query);
}