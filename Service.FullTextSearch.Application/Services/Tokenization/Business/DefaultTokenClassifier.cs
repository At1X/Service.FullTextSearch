using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Application.Services.Tokenization.Business;

public class DefaultTokenClassifier : ITokenClassifier
{
    public TokenClassification Classify(string token)
    {
        return token[0] switch
        {
            '+' => new TokenClassification(TokenType.Optional, token.Substring(1)),
            '-' => new TokenClassification(TokenType.Excluded, token.Substring(1)),
            _ => new TokenClassification(TokenType.Required, token)
        };
    }
}