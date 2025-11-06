using MediatR;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Domain.Interfaces;

namespace Service.FullTextSearch.Application.Documents.Commands;

public class IndexDocumentCommandHandler : IRequestHandler<IndexDocumentCommand, Result<Guid>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly ITokenizer _tokenizer;
    private readonly IStopWordRemover _stopWordRemover;

    public IndexDocumentCommandHandler(
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

    public async Task<Result<Guid>> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var document = new Document(
                request.Title, 
                request.Content);

            _documentRepository.Add(document);

            var tokens = _tokenizer.Tokenize(request.Content);
            var cleanTokens = _stopWordRemover.RemoveStopWords(tokens);

            var termFrequency = cleanTokens
                .GroupBy(t => t.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (term, frequency) in termFrequency)
            {
                var index = _indexRepository.GetByTerm(term);
                
                if (index == null)
                {
                    index = new InvertedIndex(term);
                    index.AddOrUpdateDocument(document.Id, frequency);
                    _indexRepository.Add(index);
                }
                else
                {
                    index.AddOrUpdateDocument(document.Id, frequency);
                    _indexRepository.Update(index);
                }
            }
            

            return Result<Guid>.Success(document.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to index document: {ex.Message}");
        }
    }
}