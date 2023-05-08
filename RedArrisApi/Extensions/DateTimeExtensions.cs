namespace RedArrisApi;

internal static class DateTimeExtensions
{
    public static string? NormalizeDateString(this string inputString)
    {
        return !DateTime.TryParse(inputString, out var date)
            ? null
            : date.ToString("yyyy-MM-dd");
    }

    public static string GetFirstDayOfYearString()
    {
        return new DateTime(DateTime.Now.Year, 1, 1)
            .ToString("yyyy-MM-dd");
    }
}