namespace Service.FullTextSearch.Application.Common.Interfaces;

public interface IStopWordRemover
{
    IEnumerable<string> RemoveStopWords(IEnumerable<string> tokens);
}