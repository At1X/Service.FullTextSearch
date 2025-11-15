namespace Service.FullTextSearch.Domain.Entities;

public record Document
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    
}