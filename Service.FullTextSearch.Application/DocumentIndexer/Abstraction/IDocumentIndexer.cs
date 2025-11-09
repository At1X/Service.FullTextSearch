namespace Service.FullTextSearch.Application.DocumentIndexer.Abstraction;

public interface IDocumentIndexer
{
    void IndexTerms(Guid documentId, IDictionary<string, int> termFrequencies);
}