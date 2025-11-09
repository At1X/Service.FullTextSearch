using MediatR;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.DocumentIndexer.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Documents.Commands;

public class IndexDocumentCommandHandler : IRequestHandler<IndexDocumentCommand, Result<Guid>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ITextProcessor _textProcessor;
    private readonly IDocumentIndexer _documentIndexer;

    public IndexDocumentCommandHandler(
        IDocumentRepository documentRepository,
        ITextProcessor textProcessor,
        IDocumentIndexer documentIndexer)
    {
        _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _textProcessor = textProcessor ?? throw new ArgumentNullException(nameof(textProcessor));
        _documentIndexer = documentIndexer ?? throw new ArgumentNullException(nameof(documentIndexer));
    }

    public async Task<Result<Guid>> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var document = new Document(request.Title, request.Content);
            _documentRepository.Add(document);

            var termFrequencies = _textProcessor.CalculateTermFrequency(request.Content);

            _documentIndexer.IndexTerms(document.Id, termFrequencies);

            return Result<Guid>.Success(document.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to index document: {ex.Message}");
        }
    }
}