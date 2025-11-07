namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IDocumentIndexer
{
    void IndexTerms(Guid documentId, IDictionary<string, int> termFrequencies);
}