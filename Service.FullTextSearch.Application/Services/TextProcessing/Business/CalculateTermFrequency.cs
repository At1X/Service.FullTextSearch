using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

namespace Service.FullTextSearch.Application.Services.TextProcessing.Business;

public class CalculateTermFrequency : ICalculateTermFrequency
{
    private readonly ITextProcessor  _textProcessor;

    public CalculateTermFrequency(ITextProcessor textProcessor)
    {
        _textProcessor = textProcessor ??  throw new ArgumentNullException(nameof(textProcessor));
    }
    
    public IDictionary<string, int> Calculate(string text)
    {
        return _textProcessor.PreProcessText(text)
            .GroupBy(term => term)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}