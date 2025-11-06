namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IDocumentParser
{
    Task<string> ParseAsync(string filePath, CancellationToken cancellationToken = default);
}