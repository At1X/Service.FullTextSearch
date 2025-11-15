namespace Service.FullTextSearch.Application.Services.Tokenization.Abstraction;

public interface ITokenizer
{
    IReadOnlyCollection<string> Tokenize(string text);
}