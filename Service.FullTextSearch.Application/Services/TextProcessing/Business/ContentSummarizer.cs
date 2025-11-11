using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

namespace Service.FullTextSearch.Application.Services.TextProcessing.Business;

public class ContentSummarizer : IContentSummarizer
{
    public string Summarize(string content, int maxLength = 200)
    {
        return content.Length > maxLength ? content[..maxLength] + "..." : content;
    }
}