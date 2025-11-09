namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IStopWordRemover
{
    IReadOnlyCollection<string> RemoveStopWords(IReadOnlyCollection<string> tokens);
}