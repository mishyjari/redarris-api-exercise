namespace RedArrisApi;

public static class DateTimeExtensions
{
    public static string? NormalizeDateString(this string inputString) =>
        DateTime.TryParse(inputString, out var date)
            ? date.ToString("yyyy-MM-dd")
            : null;

    public static string GetFirstDayOfYearString() =>
        new DateTime(DateTime.Now.Year, 1, 1)
            .ToString("yyyy-MM-dd");
}