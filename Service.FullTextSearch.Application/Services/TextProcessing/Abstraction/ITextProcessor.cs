namespace Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

public interface ITextProcessor
{
    IReadOnlyCollection<string> PreProcessText(string text);
}
