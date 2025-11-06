namespace Service.FullTextSearch.Application.Documents.Models;

public class ScoredDocument
{
    public Guid DocumentId { get; }
    public int Score { get; }

    public ScoredDocument(Guid documentId, int score)
    {
        if (documentId == Guid.Empty)
            throw new ArgumentException("Document ID cannot be empty", nameof(documentId));

        if (score < 0)
            throw new ArgumentException("Score cannot be negative", nameof(score));

        DocumentId = documentId;
        Score = score;
    }
}