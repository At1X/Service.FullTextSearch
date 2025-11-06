namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ITokenizer
{
    IEnumerable<string> Tokenize(string text);
}