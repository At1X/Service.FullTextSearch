using System.Text.RegularExpressions;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Configurations;

namespace Service.FullTextSearch.Application.Services.Tokenization.Business;

public class Tokenizer: ITokenizer
{
    public IReadOnlyCollection<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Enumerable.Empty<string>().ToList();

        var normalized = text.ToLowerInvariant();
        
        var tokens = Regex.Split(normalized, FullTextSearchConfig.TokenizeSplitterRegex)
            .Where(t => !string.IsNullOrWhiteSpace(t) && t.Length > 1).ToList();

        return tokens;
    }
}