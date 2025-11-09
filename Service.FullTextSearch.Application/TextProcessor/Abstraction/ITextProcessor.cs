namespace Service.FullTextSearch.Application.TextProcessor.Abstraction;

public interface ITextProcessor
{
    IReadOnlyCollection<string> PreProcessText(string text);
    IDictionary<string, int> CalculateTermFrequency(string text);
}
