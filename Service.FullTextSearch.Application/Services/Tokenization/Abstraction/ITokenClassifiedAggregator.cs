using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Tokenization.Abstraction;

public interface ITokenClassifiedAggregator
{
    ParsedQuery Aggregate(IReadOnlyCollection<TokenClassification> classifications);
}