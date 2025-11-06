using MediatR;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Interfaces;

namespace Service.FullTextSearch.Application.Documents.Queries;

public class SearchTermQueryHandler : IRequestHandler<SearchTermQuery, Result<SearchResultDto>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly ITokenizer _tokenizer;
    private readonly IStopWordRemover _stopWordRemover;

    public SearchTermQueryHandler(
        IDocumentRepository documentRepository,
        IInvertedIndexRepository indexRepository,
        ITokenizer tokenizer,
        IStopWordRemover stopWordRemover)
    {
        _documentRepository = documentRepository;
        _indexRepository = indexRepository;
        _tokenizer = tokenizer;
        _stopWordRemover = stopWordRemover;
    }

    public async Task<Result<SearchResultDto>> Handle(SearchTermQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tokens = _tokenizer.Tokenize(request.SearchText);
            var cleanTokens = _stopWordRemover.RemoveStopWords(tokens).ToList();

            if (!cleanTokens.Any())
                return Result<SearchResultDto>.Success(new SearchResultDto(
                    new List<DocumentResultDto>(), 0, request.SearchText));

            var indices = _indexRepository.SearchTerms(
                cleanTokens.Select(t => t.ToLowerInvariant()));

            var documentScores = new Dictionary<Guid, int>();
            foreach (var index in indices)
            {
                foreach (var docId in index.GetDocumentIds())
                {
                    var frequency = index.GetFrequency(docId);
                    if (documentScores.ContainsKey(docId))
                        documentScores[docId] += frequency;
                    else
                        documentScores[docId] = frequency;
                }
            }

            var results = new List<DocumentResultDto>();
            foreach (var (docId, score) in documentScores.OrderByDescending(x => x.Value))
            {
                var doc = _documentRepository.GetById(docId);
                if (doc != null)
                {
                    results.Add(new DocumentResultDto(
                        doc.Id,
                        doc.Title,
                        doc.Content.Length > 200 ? doc.Content[..200] + "..." : doc.Content,
                        score));
                }
            }

            return Result<SearchResultDto>.Success(new SearchResultDto(
                results,
                results.Count,
                request.SearchText));
        }
        catch (Exception ex)
        {
            return Result<SearchResultDto>.Failure($"Search failed: {ex.Message}");
        }
    }
}