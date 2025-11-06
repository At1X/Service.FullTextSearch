using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Domain.Interfaces;

public interface IDocumentRepository
{
    Document? GetById(Guid id);
    IReadOnlyList<Document> GetAll();
    Document Add(Document document);
    void Update(Document document);
    void Delete(Guid id);
}