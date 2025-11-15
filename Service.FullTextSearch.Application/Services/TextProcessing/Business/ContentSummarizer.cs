using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;
using Service.FullTextSearch.Domain.Configurations;

namespace Service.FullTextSearch.Application.Services.TextProcessing.Business;

public class ContentSummarizer : IContentSummarizer
{
    public string Summarize(string content, int? maxLength)
    {
        var filledMaxLength = maxLength ?? FullTextSearchConfig.ContentSummrizerDefaultLength;
        return content.Length > filledMaxLength ? content[..filledMaxLength] + "..." : content;
    }
}