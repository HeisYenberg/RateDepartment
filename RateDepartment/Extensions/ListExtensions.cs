namespace RateDepartment.Extensions;

public static class ListExtensions
{
    public static string Join(this IEnumerable<string> list, string separator) => string.Join(separator, list);
}