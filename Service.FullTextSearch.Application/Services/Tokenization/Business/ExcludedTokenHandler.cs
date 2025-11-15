using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Extensions;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Enums;

namespace Service.FullTextSearch.Application.Services.Tokenization.Business;

public class ExcludedTokenHandler : ITokenHandler
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentRetriever  _indexDocumentRetriever;

    public ExcludedTokenHandler(IInvertedIndexRepository indexRepository,  IInvertedIndexDocumentRetriever indexDocumentRetriever)
    {
        _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        _indexDocumentRetriever = indexDocumentRetriever ?? throw new ArgumentNullException(nameof(indexDocumentRetriever));
    }
    public bool CanHandle(TokenType type) => type == TokenType.Excluded;
    public void HandleParser(ParsedQuery query, string term) => query.ExcludedTerms.AddDistinct(term);
    
    public IReadOnlyCollection<ScoredDocument> FilterHandler(
        IReadOnlyCollection<ScoredDocument> documents,
        ParsedQuery query)
    
    {
        ArgumentNullException.ThrowIfNull(documents);
        ArgumentNullException.ThrowIfNull(query);
        
        if (!query.HasExcludedTerms)
        {
            return documents;
        }
        
        var excludedIndices = _indexRepository.SearchTerms(query.ExcludedTerms);

        var excludedDocumentIds = excludedIndices
            .SelectMany(idx => _indexDocumentRetriever.GetDocumentIds(idx))
            .Distinct()
            .ToHashSet();

        return documents
            .Where(d => !excludedDocumentIds.Contains(d.DocumentId))
            .ToList();
    }
}