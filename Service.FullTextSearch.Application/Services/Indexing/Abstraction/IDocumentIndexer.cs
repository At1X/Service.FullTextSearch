namespace Service.FullTextSearch.Application.Services.Indexing.Abstraction;

public interface IDocumentIndexer
{
    void IndexTerms(Guid documentId, IDictionary<string, int> termFrequencies);
}