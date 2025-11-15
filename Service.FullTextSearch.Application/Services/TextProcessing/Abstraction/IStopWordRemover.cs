namespace Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;

public interface IStopWordRemover
{
    IReadOnlyCollection<string> Remove(IReadOnlyCollection<string> tokens);
}