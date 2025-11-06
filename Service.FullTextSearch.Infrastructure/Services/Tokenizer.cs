using System.Text.RegularExpressions;
using Service.FullTextSearch.Application.Common.Interfaces;

namespace Service.FullTextSearch.Infrastructure.Services;

public class Tokenizer: ITokenizer
{
    public IEnumerable<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Enumerable.Empty<string>();

        var normalized = text.ToLowerInvariant();
        
        var tokens = Regex.Split(normalized, @"\W+")
            .Where(t => !string.IsNullOrWhiteSpace(t) && t.Length > 1);

        return tokens;
    }
}