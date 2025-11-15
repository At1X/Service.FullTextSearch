using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;

namespace Service.FullTextSearch.Application.Services.TextProcessing.Business;

public class StandardTextProcessor : ITextProcessor
{
    private readonly ITokenizer _tokenizer;
    private readonly IStopWordRemover _stopWordRemover;

    public StandardTextProcessor(ITokenizer tokenizer, IStopWordRemover stopWordRemover)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _stopWordRemover = stopWordRemover ??  throw new ArgumentNullException(nameof(stopWordRemover));
    }

    public IReadOnlyCollection<string> PreProcessText(string text)
    {
        var tokens = _tokenizer.Tokenize(text);
        var cleaned = _stopWordRemover.Remove(tokens);
        return cleaned.Select(t => t.ToLowerInvariant()).ToList();
    }
    
}