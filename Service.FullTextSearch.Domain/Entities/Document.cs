using Service.FullTextSearch.Domain.Common;

namespace Service.FullTextSearch.Domain.Entities;

public class Document : BaseEntity
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string FilePath { get; private set; }

    private Document() { } // For ORM

    public Document(string title, string content, string filePath)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));

        Title = title;
        Content = content;
        FilePath = filePath ?? string.Empty;
    }

    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Content cannot be empty", nameof(newContent));

        Content = newContent;
        SetUpdated();
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty", nameof(newTitle));

        Title = newTitle;
        SetUpdated();
    }
}