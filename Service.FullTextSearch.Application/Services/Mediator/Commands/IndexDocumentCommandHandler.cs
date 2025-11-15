using MediatR;
using Service.FullTextSearch.Application.Common.Builders;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

namespace Service.FullTextSearch.Application.Services.Mediator.Commands;

public class IndexDocumentCommandHandler : IRequestHandler<IndexDocumentCommand, Result<Guid>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentIndexer _documentIndexer;
    private readonly ICalculateTermFrequency  _calculateTermFrequency;

    public IndexDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IDocumentIndexer documentIndexer,
        ICalculateTermFrequency calculateTermFrequency)
    {
        _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _documentIndexer = documentIndexer ?? throw new ArgumentNullException(nameof(documentIndexer));
        _calculateTermFrequency = calculateTermFrequency ?? throw new ArgumentNullException(nameof(calculateTermFrequency));
    }

    public async Task<Result<Guid>> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var document = new DocumentBuilder()
                .WithTitle(request.Title)
                .WithContent(request.Content)
                .Build();
            _documentRepository.Add(document);

            var termFrequencies = _calculateTermFrequency.Calculate(request.Content);

            _documentIndexer.IndexTerms(document.Id, termFrequencies);

            return Result<Guid>.Success(document.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to index document: {ex.Message}");
        }
    }
}