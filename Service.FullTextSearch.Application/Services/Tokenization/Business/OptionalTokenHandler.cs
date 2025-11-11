using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Extensions;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Application.Services.Tokenization.Business;

public class OptionalTokenHandler : ITokenHandler
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentRetriever  _indexDocumentRetriever;

    public OptionalTokenHandler(IInvertedIndexRepository indexRepository, IInvertedIndexDocumentRetriever indexDocumentRetriever)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _indexDocumentRetriever = indexDocumentRetriever ?? throw new ArgumentNullException(nameof(indexDocumentRetriever));
    }
    public bool CanHandle(TokenType type) => type == TokenType.Optional;
    public void HandleParser(ParsedQuery query, string term) => query.OptionalTerms.AddDistinct(term);
    
    public IReadOnlyCollection<ScoredDocument> FilterHandler(
        IReadOnlyCollection<ScoredDocument> documents,
        ParsedQuery query)
    {
        if (!query.HasOptionalTerms)
        {
            return documents;
        }

        var optionalIndices = _indexRepository.SearchTerms(query.OptionalTerms);
        
        if (!optionalIndices.Any())
        {
            return Array.Empty<ScoredDocument>();
        }

        var validDocumentIds = optionalIndices
            .SelectMany(idx => _indexDocumentRetriever.GetDocumentIds(idx))
            .Distinct()
            .ToHashSet();

        return documents
            .Where(d => validDocumentIds.Contains(d.DocumentId))
            .ToList();
    }
}