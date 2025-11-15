namespace Service.FullTextSearch.Domain.Entities;

public class ParsedQuery
{
    public HashSet<string> RequiredTerms { get; init; } = [];
    public HashSet<string> OptionalTerms { get; init; } = [];
    public HashSet<string> ExcludedTerms { get; init; } = [];

    public bool HasOptionalTerms => OptionalTerms.Count > 0;
    public bool HasExcludedTerms => ExcludedTerms.Count > 0;
}