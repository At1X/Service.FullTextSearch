using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.ContentSummerizer.Abstraction;
using Service.FullTextSearch.Application.SearchResultMapper.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.SearchResultMapper.Business;

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