namespace Service.FullTextSearch.Application.Extensions;

public static class CollectionExtensions
{
    public static void AddDistinct(this HashSet<string> hashSet, string item)
    {
        hashSet.Add(item);
    }
}