namespace Service.FullTextSearch.Application.StopWordRemover.Abstraction;

public interface IStopWordRemover
{
    IReadOnlyCollection<string> RemoveStopWords(IReadOnlyCollection<string> tokens);
}