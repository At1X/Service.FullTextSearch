using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Documents.DTOs;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Infrastructure.Services;

public class DocumentResultMapper : ISearchResultMapper
{
    private readonly IContentSummarizer _summarizer;

    public DocumentResultMapper(IContentSummarizer summarizer)
    {
        _summarizer = summarizer ?? throw new ArgumentNullException(nameof(summarizer));
    }

    public DocumentResultDto MapToDto(Document document, int score)
    {
        var summary = _summarizer.Summarize(document.Content, maxLength: 200);
        return new DocumentResultDto(
            document.Id,
            document.Title,
            summary,
            score);
    }
}