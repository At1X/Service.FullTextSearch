namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface ITextProcessor
{
    IEnumerable<string> Process(string text);
}
