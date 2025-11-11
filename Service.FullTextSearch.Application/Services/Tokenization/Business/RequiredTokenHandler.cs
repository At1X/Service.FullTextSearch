using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Extensions;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Application.Services.Tokenization.Business;

public class RequiredTokenHandler : ITokenHandler
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentRetriever _indexDocumentRetriever;

    public RequiredTokenHandler(IInvertedIndexRepository indexRepository,  IInvertedIndexDocumentRetriever indexDocumentRetriever)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _indexDocumentRetriever = indexDocumentRetriever ?? throw new ArgumentNullException(nameof(indexDocumentRetriever));
    }
    public bool CanHandle(TokenType type) => type == TokenType.Required;
    public void HandleParser(ParsedQuery query, string term) => query.RequiredTerms.AddDistinct(term);
    
    public IReadOnlyCollection<ScoredDocument> FilterHandler(
        IReadOnlyCollection<ScoredDocument> documents,
        ParsedQuery query)
    {
        if (!query.RequiredTerms.Any())
        {
            return documents;
        }

        var requiredIndices = _indexRepository.SearchTerms(query.RequiredTerms);

        var documentSets = requiredIndices
            .Select(idx => _indexDocumentRetriever.GetDocumentIds(idx))
            .ToList();

        var validDocumentIds = documentSets
            .Skip(1)
            .Aggregate(
                new HashSet<Guid>(documentSets.First()),
                (set, next) =>
                {
                    set.IntersectWith(next);
                    return set;
                });

        return documents
            .Where(d => validDocumentIds.Contains(d.DocumentId))
            .ToList();
    }
}