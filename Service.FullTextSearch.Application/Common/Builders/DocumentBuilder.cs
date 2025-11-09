using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Common.Builders;

public class DocumentBuilder
{
    private string _title;
    private string _content;
    
    public DocumentBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public DocumentBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }
    
    public Document Build()
    {
        if (string.IsNullOrWhiteSpace(_title))
            throw new ArgumentException("Title cannot be empty", nameof(_title));
        
        if (string.IsNullOrWhiteSpace(_content))
            throw new ArgumentException("Content cannot be empty", nameof(_content));
        
        return new Document
        {
            Id = Guid.NewGuid(),
            Title = _title,
            Content = _content
        };
    }
}