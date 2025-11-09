namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ITokenizer
{
    IReadOnlyCollection<string> Tokenize(string text);
}