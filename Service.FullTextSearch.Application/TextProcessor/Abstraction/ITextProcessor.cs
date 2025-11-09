namespace Service.FullTextSearch.Application.TextProcessor.Abstraction;

public interface ITextProcessor
{
    IReadOnlyCollection<string> Process(string text);
    IDictionary<string, int> CalculateTermFrequency(string text);
}
