using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Tokenization.Abstraction;

public interface ITokenClassifier
{
    TokenClassification Classify(string token);
}