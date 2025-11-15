namespace Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

public interface ICalculateTermFrequency
{
    IDictionary<string, int> Calculate(string text);
    
}