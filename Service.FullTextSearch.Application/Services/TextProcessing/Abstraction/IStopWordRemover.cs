namespace Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

public interface IStopWordRemover
{
    IReadOnlyCollection<string> RemoveStopWords(IReadOnlyCollection<string> tokens);
}