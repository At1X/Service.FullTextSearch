using Service.FullTextSearch.Application.Common.Interfaces;

namespace Service.FullTextSearch.Infrastructure.Services;

public class StandardTextProcessor : ITextProcessor
{
    private readonly ITokenizer _tokenizer;
    private readonly IStopWordRemover _stopWordRemover;

    public StandardTextProcessor(ITokenizer tokenizer, IStopWordRemover stopWordRemover)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _stopWordRemover = stopWordRemover ??  throw new ArgumentNullException(nameof(stopWordRemover));
    }

    public IEnumerable<string> Process(string text)
    {
        var tokens = _tokenizer.Tokenize(text);
        var cleaned = _stopWordRemover.RemoveStopWords(tokens);
        return cleaned.Select(t => t.ToLowerInvariant());
    }
    
    public Dictionary<string, int> CalculateTermFrequency(string text)
    {
        return Process(text)
            .GroupBy(term => term)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}