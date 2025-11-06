using Service.FullTextSearch.Application.Common.Interfaces;

namespace Service.FullTextSearch.Infrastructure.Services;

public class ContentSummarizer : IContentSummarizer
{
    public string Summarize(string content, int maxLength = 200)
    {
        return content.Length > maxLength ? content[..maxLength] + "..." : content;
    }
}