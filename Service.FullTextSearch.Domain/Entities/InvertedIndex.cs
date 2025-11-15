namespace Service.FullTextSearch.Domain.Entities;

public record InvertedIndex
{
    public required Guid Id { get; set; }
    public required string Term { get; set; }
    public required Dictionary<Guid, int> DocumentFrequency { get; set; }
}