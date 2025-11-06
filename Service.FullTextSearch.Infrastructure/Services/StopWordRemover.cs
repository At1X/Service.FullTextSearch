using Service.FullTextSearch.Application.Common.Interfaces;

namespace Service.FullTextSearch.Infrastructure.Services;

public class StopWordRemover: IStopWordRemover
{
    private readonly HashSet<string> _stopWords;

    public StopWordRemover()
    {
        _stopWords = new HashSet<string>
        {
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "will", "with", "the", "this", "but", "they", "have",
            "had", "what", "when", "where", "who", "which", "why", "how"
        };
    }

    public IEnumerable<string> RemoveStopWords(IEnumerable<string> tokens)
    {
        return tokens.Where(t => !_stopWords.Contains(t.ToLowerInvariant()));
    }
}