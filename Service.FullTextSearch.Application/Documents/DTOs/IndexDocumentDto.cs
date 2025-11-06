namespace Service.FullTextSearch.Application.Documents.DTOs;

public class IndexDocumentDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string? FilePath { get; set; }
}