namespace Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

public interface IContentSummarizer
{
    string Summarize(string content, int maxLength);
}