namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IContentSummarizer
{
    string Summarize(string content, int maxLength);
}