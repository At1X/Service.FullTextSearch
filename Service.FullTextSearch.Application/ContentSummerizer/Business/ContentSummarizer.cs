using Service.FullTextSearch.Application.ContentSummerizer.Abstraction;

namespace Service.FullTextSearch.Application.ContentSummerizer.Business;

public class ContentSummarizer : IContentSummarizer
{
    public string Summarize(string content, int maxLength = 200)
    {
        return content.Length > maxLength ? content[..maxLength] + "..." : content;
    }
}