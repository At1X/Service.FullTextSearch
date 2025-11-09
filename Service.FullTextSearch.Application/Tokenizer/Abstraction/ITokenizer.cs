namespace Service.FullTextSearch.Application.Tokenizer.Abstraction;

public interface ITokenizer
{
    IReadOnlyCollection<string> Tokenize(string text);
}