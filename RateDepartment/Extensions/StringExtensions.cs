namespace RateDepartment.Extensions;

public static class StringExtensions
{
    public static string Format(this string format, params object[] args) => string.Format(format, args);
}