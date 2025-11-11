namespace Service.FullTextSearch.Domain.Entities;

public class ParsedQuery
{
    public List<string> RequiredTerms { get; init; } = new List<string>();
    public List<string> OptionalTerms { get; init; } = new List<string>();
    public List<string> ExcludedTerms { get; init; } = new List<string>();

    public bool HasOptionalTerms => OptionalTerms.Any();
    public bool HasExcludedTerms => ExcludedTerms.Any();
}