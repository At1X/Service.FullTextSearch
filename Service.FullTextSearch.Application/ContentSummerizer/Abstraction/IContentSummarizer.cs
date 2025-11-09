namespace Service.FullTextSearch.Application.ContentSummerizer.Abstraction;

public interface IContentSummarizer
{
    string Summarize(string content, int maxLength);
}