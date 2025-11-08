using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Infrastructure.Persistence.Repositories;

public class InMemoryDocumentRepository : IDocumentRepository
{
    private readonly Dictionary<Guid, Document> _documents = new();

    public Document? GetById(Guid id)
    {
        _documents.TryGetValue(id, out var document);
        return document;
    }

    public IReadOnlyList<Document> GetAll()
    {
        return _documents.Values.ToList();
    }

    public Document Add(Document document)
    {
        _documents[document.Id] = document;
        return document;
    }

    public void Update(Document document)
    {
        _documents[document.Id] = document;
    }

    public void Delete(Guid id)
    {
        _documents.Remove(id);
    }
}