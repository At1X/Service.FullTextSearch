namespace Service.FullTextSearch.Domain.Entities;

public record ScoredDocument
{
    public Guid DocumentId { get; init; }
    public int Score { get; init;  }
}