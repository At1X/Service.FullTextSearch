namespace Service.FullTextSearch.Domain.Entities;

public class ScoredDocument
{
    public Guid DocumentId { get; set; }
    public int Score { get; set;  }
}