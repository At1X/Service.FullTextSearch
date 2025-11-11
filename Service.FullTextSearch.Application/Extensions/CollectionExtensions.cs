namespace Service.FullTextSearch.Application.Extensions;

public static class CollectionExtensions
{
    public static void AddDistinct(this ICollection<string> collection, string item)
    {
        if (!collection.Contains(item))
            collection.Add(item);
    }
}