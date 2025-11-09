namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ITextProcessor
{
    IReadOnlyCollection<string> Process(string text);
    IDictionary<string, int> CalculateTermFrequency(string text);
}
