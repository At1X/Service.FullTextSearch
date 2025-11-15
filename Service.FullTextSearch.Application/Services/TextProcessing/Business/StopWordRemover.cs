using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

namespace Service.FullTextSearch.Application.Services.TextProcessing.Business;

public class StopWordRemover : IStopWordRemover
{
    private readonly HashSet<string> _stopWords;

    public StopWordRemover()
    {
        _stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "will", "with", "this", "but", "they", "have",
            "had", "what", "when", "where", "who", "which", "why", "how"
        };
    }

    public IReadOnlyCollection<string> Remove(IReadOnlyCollection<string> tokens)
    {
        return tokens.Except(_stopWords, StringComparer.OrdinalIgnoreCase).ToList();
    }
}